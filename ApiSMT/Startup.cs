using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ControleEPI.DTO._DbContext;
using Vestimenta.DTO._DbContext;
using Vestimenta.DAL;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using ApiSMT.Utilitários;
using System.Reflection;
using System.IO;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiSMT.Utilitários.JWT;
using DinkToPdf.Contracts;
using DinkToPdf;
using ApiSMT.Utilitários.email;
using ControleEPI.BLL.Categorias;
using ControleEPI.DAL.Categorias;
using ControleEPI.DAL.Produtos;
using ControleEPI.BLL.Produtos;
using ControleEPI.BLL.Certificado;
using ControleEPI.DAL.Certificado;
using ControleEPI.BLL.Compras;
using ControleEPI.BLL.Fornecedores;
using ControleEPI.BLL.LogCompras;
using ControleEPI.BLL.LogEstoque;
using ControleEPI.BLL.Motivos;
using ControleEPI.BLL.PedidosAprovados;
using ControleEPI.BLL.Pedidos;
using ControleEPI.BLL.ProdutosEstoque;
using ControleEPI.BLL.Status;
using ControleEPI.BLL.Tamanhos;
using ControleEPI.BLL.Vinculo;
using ControleEPI.BLL.RHCargos;
using ControleEPI.BLL.RHUsuarios;
using ControleEPI.BLL.RHDepartamentos;
using ControleEPI.BLL.RHContratos;
using ControleEPI.DAL.EPICompras;
using ControleEPI.DAL.EPIFornecedores;
using ControleEPI.DAL.EPILogCompras;
using ControleEPI.DAL.EPILogEstoque;
using ControleEPI.DAL.EPIMotivos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIVinculos;
using ControleEPI.DAL.RHCargos;
using ControleEPI.DAL.RHUsuarios;
using ControleEPI.DAL.RHDepartamentos;
using ControleEPI.DAL.RHContratos;

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

            services.AddDbContextPool<AppDbContext>(options => options.UseMySql(SMTConnection, ServerVersion.AutoDetect(SMTConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));
            services.AddDbContextPool<AppDbContextRH>(options => options.UseMySql(RHConnection, ServerVersion.AutoDetect(RHConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));
            services.AddDbContextPool<VestAppDbContext>(options => options.UseMySql(SMTConnection, ServerVersion.AutoDetect(SMTConnection))
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)));

            services.Configure<EmailSettingsDTO>(Configuration.GetSection("EmailSettings"));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            //EPI
            services.AddScoped<IEPICategoriasDAL, EPICategoriasDAL>();
            services.AddScoped<IEPICategoriasBLL, EPICategoriasBLL>();

            services.AddScoped<IEPICertificadoAprovacaoDAL, EPICertificadoAprovacaoDAL>();
            services.AddScoped<IEPICertificadoAprovacaoBLL, EPICertificadoAprovacaoBLL>();
            services.AddScoped<IEPIComprasBLL, EPIComprasDAL>();
            services.AddScoped<IEPIFornecedoresBLL, EPIFornecedoresDAL>();
            services.AddScoped<IEPILogComprasBLL, EPILogComprasDAL>();
            services.AddScoped<IEPILogEstoqueBLL, EPILogEstoqueDAL>();
            services.AddScoped<IEPIMotivosBLL, EPIMotivosDAL>();
            services.AddScoped<IEPIPedidosAprovadosBLL, EPIPedidosAprovadosDAL>();

            services.AddScoped<IEPIProdutosDAL, EPIProdutosDAL>();
            services.AddScoped<IEPIProdutosBLL, EPIProdutosBLL>();

            services.AddScoped<IEPIPedidosBLL, EPIPedidosDAL>();
            services.AddScoped<IEPIProdutosEstoqueBLL, EPIProdutosEstoqueDAL>();
            services.AddScoped<IEPIStatusBLL, EPIStatusDAL>();
            services.AddScoped<IEPITamanhosBLL, EPITamanhosDAL>();
            services.AddScoped<IEPIVinculoBLL, EPIVinculoDAL>();
            services.AddScoped<IRHCargosBLL, RHCargosDAL>();  
            services.AddScoped<IRHConUserBLL, RHConUserDAL>();
            services.AddScoped<IRHDepartamentosBLL, RHDepartamentosDAL>();
            services.AddScoped<IRHEmpContratosBLL, RHEmpContratosDAL>();

            //E-Mail
            services.AddScoped<IMailService, MailService>();

            //Vestimenta
            services.AddScoped<IVestimentaBLL, VestimentaDAL>();
            services.AddScoped<IComprasVestBLL, ComprasVestDAL>();
            services.AddScoped<IStatusVestBLL, StatusVestDAL>();
            services.AddScoped<IPedidosVestBLL, PedidosVestDAL>();
            services.AddScoped<IEstoqueBLL, EstoqueDAL>();
            services.AddScoped<ILogBLL, LogDAL>();
            services.AddScoped<IVestVinculoBLL, VestVinculoDAL>();
            services.AddScoped<IVestRepositorioBLL, VestRepositorioDAL>();
            services.AddScoped<IDinkPDFBLL, DinkPDFDAL>();

            services.AddControllers();
            services.AddHostedService<TimerHostedService>();

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
