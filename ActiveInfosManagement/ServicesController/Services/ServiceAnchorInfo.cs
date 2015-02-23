using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using GraphicsService.ActionManager;
using GraphicsService.ActiveInfosManagement;
using GraphicsService.Graphism.Graphics;

using TSFW = Dorna.Timekeeping.TunasFrameworkNET.SF;
using RenderEngine.Events;
using RenderEngine;

using cvfn;

namespace GraphismVAS0.ActiveInfosManagement.ServicesController.Services
{
    class ServiceAnchorInfo
    {
        Timer Clock;
        IActionManager mActionManager;
        RenderEngine.IRenderEngine mRenderEngine;
        InfoData mTheInfo;
        IComputerVisionManager mcvManager;


        // ------------------------------------------------------------------------------------------------------------------
        public ServiceAnchorInfo(   InfoData pTheInfo,
                                    int dataFrameRate,
                                    IActionManager actionManager,
                                    RenderEngine.IRenderEngine renderEngine,
                                    IComputerVisionManager cvManager )
        {
            const int TIMETOSTART = 0;
            int iDelay = 1000 / dataFrameRate;
            Clock = new Timer(readLiveData, null, TIMETOSTART, iDelay);
            mActionManager = actionManager;
            mTheInfo = pTheInfo;
            mRenderEngine = renderEngine;
            mcvManager = cvManager;
        }

        // ------------------------------------------------------------------------------------------------------------------
        protected void readLiveData(object pState)
        {
            lock (this)
            {
                try {  
                    sendData();   
                }
                catch  {
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------
        private void sendData()
        {
            IGCOutput GCOutput = mActionManager.GetGraphicControllerOutputOrDefault(mTheInfo.NameOutput);
            Dictionary<string, string> Tags = new Dictionary<string, string>();
            
            if (mcvManager != null)
            {
                SingleFeatureTrackerCtrl tracker = new SingleFeatureTrackerCtrl();
                mcvManager.getFrameProcessor(tracker);
                if (tracker != null)
                {

                    Point2D pt = tracker.getNormalizedTrackingPoint();
                    Point2D atc = tracker.getNormalizedSecondaryAttachedPoint();

                    if (GCOutput != null)
                    {

                        Thread.Sleep((int)GraphismVAS2015_0.SendDataDelay);

                        GCOutput._AddTag(mTheInfo, GraphismVAS0KeyWords.Data, pt.ToString("N4",";") + ";" + atc.ToString("N4",";"), null, ref Tags);
                        foreach (var tag in Tags)
                        {
                            mRenderEngine.NewValueHandler(this, new NewOutValueArgs()
                            {
                                Name = tag.Key,
                                Output = new OutputDesc() { Name = mTheInfo.NameOutput },
                                Value = tag.Value
                            });
                        }
                    }
                }
            }

        }

        // ------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            Clock.Dispose();
            mActionManager = null;
        }

    }
}
