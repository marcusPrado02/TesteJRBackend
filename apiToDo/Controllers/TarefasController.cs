using apiToDo.DTO;
using apiToDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace apiToDo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefasController : ControllerBase
    {
        // Intranciando a classe Tarefas para acesso
        private Tarefas acessoTarefas;


        // Construtor de TarefasController  
        public TarefasController()
        {
            acessoTarefas = new Tarefas(); ;
        }


        /// <summary>
        /// Método do Controller de listagem de tarefas cadastradas no sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet("lstTarefas")]
        public ActionResult<IEnumerable<TarefaDTO>> lstTarefas()
        {
            try
            {


                /*
                 * Chamamos o método de listar as todas as Tarefas
                 * cadastradas e em seguida testamos se a lista 
                 * retornada é nula ou não
                 */
                List<TarefaDTO> listaTarefas = acessoTarefas.lstTarefas();
                bool testeNull = listaTarefas == null;

                /*
                 * Se testeNull tiver valor verdadeiro então retornamos
                 * o código 400 juntamente com uma mensagem de erro
                 */
                if (testeNull)
                {
                    return BadRequest("Nenhuma Tarefa está cadastrada atualmente, tente novamente mais tarde.");
                }

                /*
                 * Caso o processo de listagem tenha ocorrida corretamente 
                 * até o momento retornamos o código 200 juntamente com a lista
                 * de tarefas
                 */
                return Ok(listaTarefas);
            }

            catch (Exception ex)
            {
                /*
                 * Caso tenha ocorrido alguma falha durante o processo
                 * de deleção retornamos o código 500 e uma mensagem
                 * de erro
                 */
                return Problem("Ocorreu um erro não foi possível listar as Tarefas cadastradas, tente novamente mais tarde.");
            }
        }


        /// <summary>
        /// Método do Controller de pesquisa de uma tarefa dentre as cadastradas no sistema
        /// </summary>
        /// <param name="ID_TAREFA"></param>
        /// <returns></returns>
        [HttpGet("PesquisarTarefas/{ID_TAREFA}")]
        public ActionResult<TarefaDTO> PesquisarTarefa(int ID_TAREFA)
        {
            try
            {

                /*
                *  Criamos uma variável booleana que utilizaremos 
                *  para avaliar se o paramêtro ID_TAREFA é negativo
                *  ou igual a zero
                */
                bool testeIdTarefa = ID_TAREFA <= 0;


                /*
                 * Se o valor da variável testeIdTarefa for igual a
                 * verdadeiro retornamos um status 400 e uma mensagem
                 * de erro, pois os números usados como identificados 
                 * começam a partir do número 1
                 */
                if (testeIdTarefa)
                {
                    return BadRequest("Ocorreu um erro ao pesquisar a Tarefa solicitada, o identificador do recurso soliciatado é inválido tente novamente mais tarde.");
                }


                /*
                * Se existir alguns itens da lista de tarefas cadastradas
                * com o ID_TAREFA igual ao solicitado retorna uma lista com 
                * esses itens
                */
                var testeEstaCadastrado = acessoTarefas.lstTarefas().Where(tarefa => tarefa.ID_TAREFA == ID_TAREFA);

                /*
                 * Testa se a lista dessas tarefas com ID igual ao passado
                 * possui algum item ou está vazia
                 */
                bool eVazio = !testeEstaCadastrado.Any();

                /*
                 * Se eVazio for igual a verdadeiro envia uma mensagem de erro 
                 * informando que não foi possível deletar a Tarefa que possuia
                 * o identificador solicitado
                 */
                if (eVazio)
                {
                    return BadRequest("Ocorreu um erro ao pesquisar a Tarefa solicitada, o identificador não existe ou foi deletado, tente novamente mais tarde.");
                }

                /*
                 * Caso o processo de listagem tenha ocorrida corretamente 
                 * até o momento retornamos o código 200 juntamente com o 
                 * objeto tarefa solicitado
                 */
                return Ok(testeEstaCadastrado.First());
            }

            catch (Exception ex)
            {
                /*
                * Caso tenha ocorrido alguma falha durante o processo
                * de deleção retornamos o código 500 e uma mensagem
                * de erro
                */
                return Problem("Ocorreu um erro não foi possível pesquisar a Tarefa solicitada, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método do Controller de inserção de tarefas cadastradas no sistema
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost("InserirTarefas")]
        public ActionResult<IEnumerable<TarefaDTO>> InserirTarefas([FromBody] TarefaDTO Request)
        {
            try
            {

                // Testamos se o parametro Request é nulo
                var testeNull = Request == null;

                /*
                 * Sendo testeNull igual a verdadeiro retornamos 
                 * o código 400 juntamente com uma mensagem de erro
                 */
                if (testeNull)
                {
                    return BadRequest(new { msg = $"Ocorreu um erro ao tentar inserir uma Tarefa com os parametros solicitados." });
                }

                /*
                 * Sendo assim executamos a função de inserir Tarefas
                 * passando Request como paramêtro
                 */
                acessoTarefas.InserirTarefa(Request);

                /*
                 * Caso tudo tenha ocorrido como o planejado
                 * retornamos o código 200 junto com a lista de todas 
                 * as tarefas cadastradas até o momento
                 */
                return Ok(acessoTarefas.lstTarefas());

            }

            catch (Exception ex)
            {
                /*
                 * Caso tenha ocorrido alguma falha durante o processo
                 * de deleção retornamos o código 500 e uma mensagem
                 * de erro
                 */
                return Problem("Ocorreu um erro durante o processo de inserção da Tarefa solicitada, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método do Controller de atualização de uma tarefa cadastrada no sistema
        /// </summary>
        /// <param name="ID_TAREFA"></param>
        /// <returns></returns>
        [HttpPut("AtualizarTarefa/{ID_TAREFA}")]
        public ActionResult<IEnumerable<TarefaDTO>> AtualizarTarefa([FromBody] TarefaDTO Request)
        {
            try
            {

                /*
                 * Para atualizarmos um registro primeiro deletamos
                 * o registro que anteriormente possuia esse identidicador
                 * e em seguida inserimos um novo registro com mesmo
                 * identificador porém atualizado
                 */
                acessoTarefas.DeletarTarefa(Request.ID_TAREFA);
                acessoTarefas.InserirTarefa(Request);

                /*
                 * Caso o processo de listagem tenha ocorrida corretamente 
                 * até o momento retornamos o código 200 juntamente com o 
                 * objeto tarefa solicitado
                 */
                return Ok(acessoTarefas.lstTarefas());
            }

            catch (Exception ex)
            {
                /*
                * Caso tenha ocorrido alguma falha durante o processo
                * de deleção retornamos o código 500 e uma mensagem
                * de erro
                */
                return BadRequest("Ocorreu um erro não foi possível atualizar a Tarefa solicitada, tente novamente mais tarde.");
            }
        }



        [HttpGet("DeletarTarefa")]
        public ActionResult DeleteTask([FromQuery] int ID_TAREFA)
        {
            try
            {

                return StatusCode(200);
            }

            catch (Exception ex)
            {
                return StatusCode(400, new { msg = $"Ocorreu um erro em sua API {ex.Message}" });
            }
        }
    }
}
