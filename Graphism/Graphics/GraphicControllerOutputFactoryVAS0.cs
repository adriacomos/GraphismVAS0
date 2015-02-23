using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService.Graphism.Exceptions;
using GraphicsService.Graphism.Graphics;

namespace GraphismVAS0.Graphism.Graphics
{
    class GraphicControllerOutputFactoryVAS0 : IGraphicControllerOutputFactory
    {

        public GraphicControllerOutputFactoryVAS0()
        {
            LstControllers = new Dictionary<string, IGCOutput>();
        }


        public IGCOutput Create(string name)
        {
            if (!LstControllers.ContainsKey(name))
                throw new GraphismException("E19200935: La salida [" + name + "] no existe en el grafismo", null);
            return LstControllers[name];
        }

        public Dictionary<string, IGCOutput> LstControllers { get; set; }
       
    }
}
