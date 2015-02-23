using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService.ActionManager;
using GraphicsService.ActiveInfosManagement;
using GraphicsService.Graphism;

using GraphismVAS0.ActiveInfosManagement.ServicesController.Services;

using cvfn;

namespace GraphismVAS0.ActiveInfosManagement.ServicesController
{
    class InfoStateControllerAnchorPoint : InfoStateControllerBasic
    {
        RenderEngine.IRenderEngine mRenderEngine;
        IComputerVisionManager  mcvManager;
        ServiceAnchorInfo mService;


        public InfoStateControllerAnchorPoint(  RenderEngine.IRenderEngine renderEngine,
                                                IComputerVisionManager  cvManager )
        {
            mRenderEngine = renderEngine;
            mcvManager = cvManager;
        }

        // ---------------------------------------------------------------------------------------------------------
        /// Función que se encarga de gestionar qué servicios hay que activar (si es necesario) ante una animación
        /// que va a ser ejecutada
        /// </summary>
        /// <param name="pTheAction"></param>        
        /// <exception cref=""> CExceptionCreatingService </exception>
        public override void ExecAction( GraphicsService.ActionManager.Action pTheAction, IActionManager actionManager)
        {
            base.ExecAction(pTheAction, actionManager);

            switch (pTheAction.CodeAction)
            {
                case KeyWords.Show:
                    ActionCommand Anim = pTheAction as ActionCommand;
                    InfoData TheInfo = Anim.TheInfo;

                    if (TheInfo != null && mService == null)
                    {
                        mService = new ServiceAnchorInfo(   TheInfo,
                                                               100, // framerate
                                                               actionManager,
                                                               mRenderEngine,
                                                               mcvManager );
                    }
                    break;
                case KeyWords.Hide:
                    if (mService != null)
                    {
                        mService.Dispose();
                        mService = null;
                    }
                    break;
            }
        }

    }
}


    