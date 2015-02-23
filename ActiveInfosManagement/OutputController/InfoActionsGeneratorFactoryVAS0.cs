using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService.ActionGeneration;

namespace GraphismVAS0.ActiveInfosManagement.OutputController
{
    class InfoActionsGeneratorFactoryVAS0 : IInfoActionsGeneratorFactory
    {
        public IInfoActionsGenerator Create(GraphicsService.ActiveInfosManagement.InfoData infoData, string outName, IInfoActionsGeneratorFactoryCreateParams aditionalParams)
        {
            return null;
        }

        public IInfoActionsGenerator Create(GraphicsService.ActiveInfosManagement.InfoData infoData, string outName)
        {
            return null;
        }
    }
}
