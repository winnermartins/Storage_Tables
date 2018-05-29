using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Actors.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/type/region/version
        [HttpGet("{region}/{version}")]
        public async Task<string> GetAsync(string region, int version)
        {
            var actor = ActorProxy.Create<IThing>(new ActorId(1), new Uri("fabric:/IoT/ThingActorService"));
            await actor.ActivateMeAsync(region, version);
            string versionString = Convert.ToString(version);
            return "Região: " + region + " - Versão: " + versionString;
        }
        
        // GET api/values/5
        /*[HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            CancellationToken cancellationToken;
            var actor = ActorProxy.Create<IActors>(new ActorId(1), new Uri("fabric:/IoT/ActorsActorService"));
            await actor.SetCountAsync(10, cancellationToken);
            var bla = actor.GetCountAsync(cancellationToken).Result;
            return Ok(bla);
        }*/

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
