﻿using ControleEPI.DTO.FromBody;
using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.Produtos
{
    public interface IEPIProdutosDAL
    {
        Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto);        
        Task<EPIProdutosDTO> localizaProduto(int id);
        Task<EPIProdutosDTO> ativaDesativaProduto(int id);
        Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado);
        Task<EPIProdutosDTO> verificaCategoria(int idCategoria);
        Task<EPIProdutosDTO> getNomeProduto(string nome);
        Task<IList<EPIProdutosDTO>> getProdutosSolicitacao();        
        Task<IList<EPIProdutosDTO>> produtosStatus(string status);
        Task<EPIProdutosDTO> Update(EPIProdutosDTO produto);
    }
}
