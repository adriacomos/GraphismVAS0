using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService.ActionGeneration;
using GraphicsService.ActiveInfosManagement;
using GraphicsService.Graphism;
using GraphicsService.Graphism.Graphics;
using GraphismVAS0.ActiveInfosManagement.OutputController;
using GraphismVAS0.ActiveInfosManagement.ServicesController;
using GraphismVAS0.Graphism.Graphics;
using RenderEngine;

using cvfn;

namespace GraphismVAS0
{

    public class GraphismVAS2015_0 : IGraphism
    {
        static public uint SendDataDelay { get; set; }

        public IInfoActionsGeneratorFactory OutputControllerFactory { get; protected set; }
        public IInfoStateControllerFactory OutputServicesControllerFactory { get; protected set; }
        public IGraphicControllerOutputFactory GraphicControllerOutputFactory { get; protected set; }

        // ------------------------------------------------------------------------------------------------------------
        public GraphismVAS2015_0( IRenderEngine renderEngine, IComputerVisionManager cvManager  )
        {
            OutputControllerFactory = new InfoActionsGeneratorFactoryVAS0();    // Generadores de acciones
            OutputServicesControllerFactory = new InfoStateControllerFactoryVAS0( renderEngine, cvManager ); // Controlador de servicios adicionales
            GraphicControllerOutputFactory = new GraphicControllerOutputFactoryVAS0();  // Generadores de animaciones
            //PythonController.Init();

        }



    }
}
