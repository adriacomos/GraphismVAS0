﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService.ActiveInfosManagement;
using RenderEngine;

using cvfn;

namespace GraphismVAS0.ActiveInfosManagement.ServicesController
{
    class InfoStateControllerFactoryVAS0 : IInfoStateControllerFactory
    {
        IRenderEngine mRenderEngine;
        IComputerVisionManager mcvManager;

        // ------------------------------------------------------------------------------------------------------------
        public InfoStateControllerFactoryVAS0(  IRenderEngine renderEngine,
                                                IComputerVisionManager cvManager )
        {
            mRenderEngine = renderEngine;
            mcvManager = cvManager;
        }


        public IInfoStateController Create(string name)
        {
            switch (name)
            {
                case GraphismVAS0KeyWords.Outputs.AnchorInfo1:
                case GraphismVAS0KeyWords.Outputs.AnchorInfo2:
                case GraphismVAS0KeyWords.Outputs.AnchorInfo3:
                    return new InfoStateControllerAnchorPoint(mRenderEngine, mcvManager);
            }
            return null;
        }
    }
}
