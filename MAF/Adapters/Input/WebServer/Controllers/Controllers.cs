using MAF.Adapters.Input.WebServer.DTO;
using MAF.Adapters.Input.WebServer.Services;
using MAF.Core.Domain;
using MAF.Ports.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAF.Adapters.Input.WebServer.Controllers;

[Route("maf/u")]

[ApiController]
public class APIControllerUser : ControllerBase
{
    private IUserInput UserInput { get; }
    public APIControllerUser(IUserInput user_input)
    {
        UserInput = user_input;
    }


    [HttpPost("login")]
    public IActionResult Login([FromForm] UserDTOReq user)
    {
        try
        {
            UserInput.GetUser(user.EmailCurrent, user.Password);

        }
        catch (Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeUser(ex);
        }

        var token = Token.GenerateToken(user.EmailCurrent);

        return Ok(token);
    }


    [HttpPost("register_user")]
    public IActionResult RegisterUser([FromForm] UserDTOReq new_user)
    {
        try
        {
            UserInput.RegisterUser(new_user.Name, new_user.EmailCurrent, new_user.Password);
        }
        catch (Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeUser(ex);
        }
        
        return Ok("Usuario registrado nessa casseta");
    }

    [Authorize]
    [HttpPut("uptd/email")]
    public IActionResult Email([FromForm] UserDTOReq user,[FromForm] string new_email)
    {
        try
        {
            UserInput.UpdateEmail(user.EmailCurrent,user.Password,new_email);
        }
        catch(Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeUser(ex);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPut("uptd/name")]
    public IActionResult Name([FromForm] UserDTOReq user,[FromForm] string new_name)
    {
        try
        {
            UserInput.UpdateName(user.EmailCurrent,user.Password,new_name);
        }
        catch(Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeUser(ex);
        }

        return Ok("Novo nome atualizado com sucesso");
    }

    [Authorize]
    [HttpPut("uptd/pw")]
    public IActionResult Password([FromForm] UserDTOReq user,[FromForm] string new_password)
    {
        try
        {
            UserInput.UpdatePW(user.EmailCurrent,user.Password,new_password);
        }
        catch(Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeUser(ex);
        }

        return Ok("Nova senha atualizada com sucesso");
    }

}



[Route("maf/t")]
[ApiController]
public class APIControllerTree : ControllerBase
{
    private ITreeInput TreeInput { get; }
    public APIControllerTree(ITreeInput tree_input)
    {
        TreeInput = tree_input;
    }

    [Authorize]
    [HttpPost("register_tree")]
    public IActionResult RegisterTree([FromForm] TreeDtoReq new_tree, IFormFile photo)
    {
        try
        {
            if (!TreeDtoReq.ValidarExtensao(photo) || !TreeDtoReq.ValidarTipoMime(photo))
            {

                return BadRequest("Tipo de arquivo inválido. Apenas JPG, PNG e JPEG são aceitos.");
            }

            TreeInput.RegisterTree(new Tree(
                new_tree.Name, new_tree.Fruit, new_tree.FkEmail,
                new_tree.SciName, new Location(new_tree.Latitude, new_tree.Longitude),
                new_tree.Description,
                new Photo(photo.OpenReadStream(), photo.ContentType,
                Path.GetExtension(photo.FileName).ToLowerInvariant()))
            );

            return Ok("Árvore registrada com sucesso");
        }
        catch (Exception ex)
        {
            return new ExceptionManager().ExceptionStatusCodeTree(ex);
        }
    }
    

    [HttpGet("{lat:double}/{lng:double}/tree")]
    public IActionResult GetByTree([FromRoute] double lat, [FromRoute] double lng)
    {
        try
        {
            Tree t = TreeInput.GetTree(new Core.Domain.Location(lat, lng));

            string url_bin = Url.Action(
                "GetByTreePhoto",
                "API",
                new { lat = t.GetLocation().GetLatitude(), lng = t.GetLocation().GetLongitude() },
                Request.Scheme
            )!;

            return Ok(TreeDtoRes.ToTreeDtoRes(t, url_bin));

        }
        catch (KeyNotFoundException)
        {

            return NotFound("Árvore não encontrada");
        }
    }


    [HttpGet("{lat:double}/{lng:double}/photo", Name = "GetByTreePhoto")]
    public IActionResult GetByTreePhoto([FromRoute] double lat, [FromRoute] double lng)
    {
        try
        {
            Photo p = TreeInput.GetTreePhoto(new Location(lat, lng));

            return File(p.GetStreamBytes(), p.GetContentType());

        }
        catch (FileNotFoundException)
        {
            return NotFound("Arquivo de foto não encontrado no servidor.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Árvore não encontrado no servidor.");
        }
        catch
        {
            return StatusCode(500, "Erro interno ao carregar a foto.");
        }
    }


    [HttpGet("get_all_trees")]
    public IActionResult GetTrees()
    {
        try
        {
            List<Tree> trees = TreeInput.GetAllTrees();

            // 1. Converte a lista de 'Tree' (domínio) para uma lista de 'TreeDtoRes' (DTO)
            var treesDto = trees.Select(t => TreeDtoRes.ToTreeDtoRes(t,
                Url.Action(
                    "GetByTreePhoto",
                    "API",
                    new { lat = t.GetLocation().GetLatitude(), lng = t.GetLocation().GetLongitude() },
                    Request.Scheme
                )!
            )).ToList();

            // 2. Retorna a lista de DTOs, que pode ser serializada corretamente
            return Ok(treesDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{lat:double}/{lng:double}/delete")]
    public IActionResult DeleteTree([FromRoute] double lat, [FromRoute] double lng)
    {
        try
        {
            if (!TreeInput.DeleteTree(new Location(lat, lng)))
            {
                throw new KeyNotFoundException();
            }

            return Ok("Árvore deletada");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}