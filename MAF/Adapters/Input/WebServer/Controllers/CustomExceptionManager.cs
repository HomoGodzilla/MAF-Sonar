using MAF.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MAF.Adapters.Input.WebServer.Controllers;

public class ExceptionManager : ControllerBase
{
    public IActionResult ExceptionStatusCodeTree(Exception ex)
    {
        return ex switch
        {
            InvalidEmailException =>
                UnprocessableEntity("Formatação do e-mail inválida"),
                
            NameOutOfRangeException =>
                BadRequest("O nome deve possuir no max 20 caracteres"),

            FruitNameOutOfRangeException =>
                BadRequest("O nome da fruta deve possuir no max 20 caracteres"),

            SciNameOutOfRangeException =>
                BadRequest("O nome cientifico deve possuir no max 20 carateres"),

            DescrOutOfRangeException =>
                BadRequest("A descrição deve possuir no max 50 caracteres"),

            _ => StatusCode(500,ex.Message),
        };

    }

    public IActionResult ExceptionStatusCodeUser(Exception ex)
    {
        return ex switch
        {
            InvalidEmailException =>
                UnprocessableEntity("Formatação do e-mail inválida"),

            NameOutOfRangeException =>
                BadRequest("O nome deve possuir no max 10 e no min 4 caracteres"),

            PasswordOutOfRangeException =>
                BadRequest("a senha deve possuir no min 8 caracteres"),
            
            KeyNotFoundException =>
                NotFound("Usuario não encontrado"),

            _ => StatusCode(500, ex.Message),
        };

    }
}


