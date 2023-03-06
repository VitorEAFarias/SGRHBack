using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiSMT.Utilitários.JWT;
using DinkToPdf.Contracts;
using DinkToPdf;
using ControleEPI.BLL.EPICategorias;
using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.BLL.EPICertificados;
using ControleEPI.DAL.EPICertificados;
using ControleEPI.BLL.EPICompras;
using ControleEPI.BLL.EPIMotivos;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIProdutosEstoque;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.BLL.EPIVinculos;
using RH.BLL.RHCargos;
using RH.BLL.RHUsuarios;
using RH.BLL.RHDepartamentos;
using RH.BLL.RHContratos;
using ControleEPI.DAL.EPICompras;
using ControleEPI.DAL.EPILogCompras;
using ControleEPI.DAL.EPILogEstoque;
using ControleEPI.DAL.EPIMotivos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIVinculos;
using RH.DAL.RHCargos;
using RH.DAL.RHUsuarios;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHContratos;
using Utilitarios.Utilitários.email;
using Utilitarios.Utilitários;
using Vestimenta.DAL.VestCompras;
using Vestimenta.DAL.VestPDF;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestLog;
using Vestimenta.DAL.VestPedidos;
using Vestimenta.DAL.VestRepositorio;
using Vestimenta.DAL.VestStatus;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DAL.VestVinculo;
using Vestimenta.BLL.VestRepositorio;
using Vestimenta.BLL.VestVinculo;
using Vestimenta.BLL.VestVestimenta;
using Vestimenta.BLL.VestCompras;
using Vestimenta.BLL.VestStatus;
using Vestimenta.BLL.VestPedidos;
using Vestimenta.BLL.VestEstoque;
using Vestimenta.BLL.VestPDF;
using ControleEPI.DAL.EPIHistorico;
using ControleEPI.BLL.EPIHistorico;

namespace ApiSMT
{
    /// <summary>
    /// Classe Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Construtor Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Interface de configuração
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Função que configura os serviços do sistema
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddTransient<ITokenService, TokenService>();

            string SMTConnection = Configuration.GetConnectionString("smt");
            string RHConnection = Configuration.GetConnectionString("rh");

            services.AddDbContextPool<ControleEPI.DTO._DbContext.AppDbContext>(options => options.UseMySql(SMTConnection, ServerVersion.AutoDetect(SMTConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));
            services.AddDbContextPool<RH.DTO._DbContext.AppDbContext>(options => options.UseMySql(RHConnection, ServerVersion.AutoDetect(RHConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));
            services.AddDbContextPool<Vestimenta.DTO._DbContext.AppDbContext>(options => options.UseMySql(SMTConnection, ServerVersion.AutoDetect(SMTConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));

            services.Configure<EmailSettingsDTO>(Configuration.GetSection("EmailSettings"));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            //EPI
            services.AddScoped<IEPICategoriasDAL, EPICategoriasDAL>();
            services.AddScoped<IEPICategoriasBLL, EPICategoriasBLL>();

            services.AddScoped<IEPICertificadoAprovacaoDAL, EPICertificadoAprovacaoDAL>();
            services.AddScoped<IEPICertificadoAprovacaoBLL, EPICertificadoAprovacaoBLL>();

            services.AddScoped<IEPIComprasDAL, EPIComprasDAL>();
            services.AddScoped<IEPIComprasBLL, EPIComprasBLL>();

            services.AddScoped<IEPILogComprasDAL, EPILogComprasDAL>();

            services.AddScoped<IEPILogEstoqueDAL, EPILogEstoqueDAL>();

            services.AddScoped<IEPIMotivosDAL, EPIMotivosDAL>();
            services.AddScoped<IEPIMotivosBLL, EPIMotivosBLL>();

            services.AddScoped<IEPIHistoricoDAL, EPIHistoricoDAL>();
            services.AddScoped<IEPIHistoricoBLL, EPIHistoricoBLL>();

            services.AddScoped<IEPIPedidosAprovadosDAL, EPIPedidosAprovadosDAL>();
            services.AddScoped<IEPIPedidosAprovadosBLL, EPIPedidosAprovadosBLL>();

            services.AddScoped<IEPIProdutosDAL, EPIProdutosDAL>();
            services.AddScoped<IEPIProdutosBLL, EPIProdutosBLL>();

            services.AddScoped<IEPIPedidosDAL, EPIPedidosDAL>();
            services.AddScoped<IEPIPedidosBLL, EPIPedidosBLL>();

            services.AddScoped<IEPIProdutosEstoqueDAL, EPIProdutosEstoqueDAL>();
            services.AddScoped<IEPIProdutosEstoqueBLL, EPIProdutosEstoqueBLL>();

            services.AddScoped<IEPIStatusDAL, EPIStatusDAL>();

            services.AddScoped<IEPITamanhosDAL, EPITamanhosDAL>();
            services.AddScoped<IEPITamanhosBLL, EPITamanhosBLL>();

            services.AddScoped<IEPIVinculoDAL, EPIVinculoDAL>();
            services.AddScoped<IEPIVinculoBLL, EPIVinculoBLL>();

            services.AddScoped<IRHCargosDAL, RHCargosDAL>();
            services.AddScoped<IRHCargosBLL, RHCargosBLL>();

            services.AddScoped<IRHConUserDAL, RHConUserDAL>();
            services.AddScoped<IRHConUserBLL, RHConUserBLL>();

            services.AddScoped<IRHDepartamentosDAL, RHDepartamentosDAL>();
            services.AddScoped<IRHDepartamentosBLL, RHDepartamentosBLL>();

            services.AddScoped<IRHEmpContratosDAL, RHEmpContratosDAL>();
            services.AddScoped<IRHEmpContratosBLL, RHEmpContratosBLL>();

            //Vestimenta
            services.AddScoped<IVestVestimentaDAL, VestVestimentaDAL>();
            services.AddScoped<IVestVestimentaBLL, VestVestimentaBLL>();

            services.AddScoped<IVestComprasDAL, VestComprasDAL>();
            services.AddScoped<IVestComprasBLL, VestComprasBLL>();

            services.AddScoped<IVestStatusDAL, VestStatusDAL>();
            services.AddScoped<IVestStatusBLL, VestStatusBLL>();

            services.AddScoped<IVestPedidosDAL, VestPedidosDAL>();
            services.AddScoped<IVestPedidosBLL, VestPedidosBLL>();

            services.AddScoped<IVestEstoqueDAL, VestEstoqueDAL>();
            services.AddScoped<IVestEstoqueBLL, VestEstoqueBLL>();

            services.AddScoped<IVestLogDAL, VestLogDAL>();

            services.AddScoped<IVestVinculoDAL, VestVinculoDAL>();
            services.AddScoped<IVestVinculoBLL, VestVinculoBLL>();

            services.AddScoped<IVestRepositorioDAL, VestRepositorioDAL>();
            services.AddScoped<IVestRepositorioBLL, VestRepositorioBLL>();

            services.AddScoped<IVestDinkPDFDAL, VestDinkPDFDAL>();
            services.AddScoped<IVestDinkPDFBLL, VestDinkPDFBLL>();

            //E-Mail
            services.AddScoped<IMailService, MailService>();

            //Heartbeat
            services.AddHostedService<TimerHostedService>();

            services.AddControllers();            

            services.AddCors(options =>
            {
                options.AddPolicy("AbertoAtodos", builder => builder                
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSMT", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer'[space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// Funçao de configuraçao de acessos a api e documentação
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiSMT v1"));
            }
            else
            {
                //app.UseHttpsRedirection();
            }

            app.UseAuthentication();

            app.UseRouting();
            app.UseCors("AbertoAtodos");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {                
                endpoints.MapControllers();
            });
        }
    }
}
