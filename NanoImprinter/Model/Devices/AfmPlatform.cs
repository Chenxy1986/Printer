using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoImprinter.Model
{
    public interface IAfmPlatform
    {
        void Run();
        //bool GoHome();
    }
    public class AfmPlatform:IAfmPlatform,IPlatform
    {
        public AfmPlatform(AfmPlatformConfig config)
        {
        }

        public void Connected()
        {
           
        }

        public void Disconnected()
        { 

        }

        public bool GoHome()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }


    public class AfmPlatformConfig
    {
    }
}
