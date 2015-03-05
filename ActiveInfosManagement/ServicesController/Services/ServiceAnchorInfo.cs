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


    class Points
    {
        public Point2D Pt;
        public Point2D SPt;
    }

    
    class ServiceAnchorInfo
    {
        IActionManager mActionManager;
        RenderEngine.IRenderEngine mRenderEngine;
        InfoData mTheInfo;
        IComputerVisionManager mcvManager;

        readonly private object mtxBuffer = new object();
        IDictionary<long, Points> mBuffer = new Dictionary<long, Points>(); //Protegido por mtxBuffer

        // --------------------------------------------------------------------------------
        class HandlerReadLiveData : GraphismVAS0.TimerServices.IEventHandlerTimer
        {
            ServiceAnchorInfo parent;

            public HandlerReadLiveData( ServiceAnchorInfo p ) 
            {
                parent = p;
            }

            public void onEvent()
            {
                parent.readLiveData(this, null);   
            }
        };

        // --------------------------------------------------------------------------------
        class HandlerSendToVIZ : GraphismVAS0.TimerServices.IEventHandlerTimer
        {
            ServiceAnchorInfo parent;

            public HandlerSendToVIZ(ServiceAnchorInfo p) 
            {
                parent = p;
            }

            public void onEvent()
            {
                parent.sendToVIZ(this, null);   
            }

        };


        HandlerReadLiveData mHandlerReadLiveData;
        HandlerSendToVIZ mHandlerSendToVIZ;
         
        // ------------------------------------------------------------------------------------------------------------------
        public ServiceAnchorInfo(   InfoData pTheInfo,
                                    int dataFrameRate,
                                    IActionManager actionManager,
                                    RenderEngine.IRenderEngine renderEngine,
                                    IComputerVisionManager cvManager )
        {
            int iDelay = 1000 / dataFrameRate;
        
            mHandlerReadLiveData = new HandlerReadLiveData(this);
            mHandlerSendToVIZ = new HandlerSendToVIZ(this);


            mActionManager = actionManager;
            mTheInfo = pTheInfo;
            mRenderEngine = renderEngine;
            mcvManager = cvManager;

            TimerServices.registerInTimer50( mHandlerReadLiveData );
            TimerServices.registerInTimer100( mHandlerSendToVIZ );

        }

        // ------------------------------------------------------------------------------------------------------------------
        //protected void readLiveData(object pState)
        protected void readLiveData( object sender,
                                  GraphismVAS0.Services.MicroLibrary.MicroTimerEventArgs timerEventArgs )
        {
            lock (this)
            {
                try {  
                    if (mcvManager != null)
                    {
                        SingleFeatureTrackerWSecondaryPt tracker = new SingleFeatureTrackerWSecondaryPt();
                        mcvManager.getFrameProcessor(tracker);
                        if (tracker != null)
                        {
                            Point2D pt = tracker.getNormalizedTrackingPoint();
                            Point2D atc = tracker.getNormalizedDeltaSecondaryPoint();

                            lock (mtxBuffer)
                            {
                                mBuffer.Add(TimerServices.MicroStopwatch.ElapsedMicroseconds, new Points() { Pt = pt, SPt = atc });
                            }
                        }
                    }
                }
                catch  {
                }
            }
        }



        // ------------------------------------------------------------------------------------------------------------------
        private void sendToVIZ(object sender, GraphismVAS0.Services.MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            try
            {
                Point2D pt = null;
                Point2D atc = null;

                long now = TimerServices.MicroStopwatch.ElapsedMicroseconds;
                long nowDel = now - GraphismVAS2015_0.SendDataDelay * 100; // 100 microseconds

                lock (mtxBuffer)
                {
                    var samples = mBuffer.Where(x => x.Key < nowDel);
                    if (samples.Any())
                    {
                        var kv = samples.First();
                        pt = kv.Value.Pt;
                        atc = kv.Value.SPt;
                        mBuffer.Remove(kv.Key);
                        //foreach (var samtodel in samples)
                        //{
                        //    mBuffer.Remove(samtodel.Key);
                        //}
                    }
                }

                if (pt != null && atc != null)
                {
                    IGCOutput GCOutput = mActionManager.GetGraphicControllerOutputOrDefault(mTheInfo.NameOutput);
                    Dictionary<string, string> Tags = new Dictionary<string, string>();
                    if (GCOutput != null)
                    {
                        GCOutput._AddTag(mTheInfo, GraphismVAS0KeyWords.Data, pt.ToString("N4", ";") + ";" + atc.ToString("N4", ";"), null, ref Tags);
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
            catch (Exception e)
            {
            }
        }

        // ------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            Timer oneTimeEvent = new Timer(endSend, null, 1000, 0);
        }


        public void endSend(object p)
        {
            mActionManager = null;

            TimerServices.unregisterInTimer50(mHandlerReadLiveData);
            TimerServices.unregisterInTimer100(mHandlerSendToVIZ);
        }

    }
}
