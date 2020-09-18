using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.IO;
using SnomedTemplateService.Parser.Generated;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace SnomedTemplateService.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TemplatesController : ControllerBase
    {
        


        private readonly ILogger<TemplatesController> _logger;
        private readonly IHostEnvironment _hostEnvironment;


        public TemplatesController(ILogger<TemplatesController> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public FileStreamResult Get()
        {
            var stream = _hostEnvironment.ContentRootFileProvider.GetFileInfo("expression-templates.json").CreateReadStream();
            return File(stream, "application/json");
        }

        public class ExpressionTemplateistener : ExpressionTemplateBaseListener
        {

        }

        [HttpGet]
        public string Test()
        {
            var lexer = new ExpressionTemplateLexer(new AntlrInputStream(_hostEnvironment.ContentRootFileProvider.GetFileInfo("test.etl").CreateReadStream()));
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();
            var parser = new ExpressionTemplateParser(tokens);
            parser.ErrorHandler = new BailErrorStrategy();
            ExpressionTemplateParser.ParseContext parseContext;
            try
            {
                parseContext = parser.parse();
            }
            catch (ParseCanceledException pce)
            {
                return "Failed \r\n" + (pce.InnerException as RecognitionException)?.Context.ToStringTree(parser);
            }
            return "Success \r\n" + parseContext.ToStringTree(parser);
            
            
                       
        }
    }
}
