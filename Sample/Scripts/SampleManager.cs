using GameServer.Core.Attribute;
using GameServer.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Scripts
{
    [GameManager(Common.NetObject.GameType.Sample)]
    public class SampleManager : IMonoInterface
    {
        public void Awake()
        {
            Console.WriteLine("SampleManager Awake");
        }

        public void LateUpdate()
        {

        }

        public void Start()
        {

        }

        public void Update()
        {

        }
    }
}
