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
using GraphismVAS0.Services.MicroLibrary;
using System.Threading;

namespace GraphismVAS0
{
    // Clase para servicios de tiempo de alta precisión
    class TimerServices
    {
        public interface IEventHandlerTimer {
            void onEvent( );
        };


        static public MicroStopwatch MicroStopwatch = new MicroStopwatch();  // Sincronismo general de tiempo 

        static readonly object mtxHndl50 = new object();
        static List<IEventHandlerTimer> mHandlers50 = new List<IEventHandlerTimer>();  // Protegido por mtxHndl50
        static readonly object mtxHndl100 = new object();
        static List<IEventHandlerTimer> mHandlers100 = new List<IEventHandlerTimer>(); // Protegido por mtxHndl100

        static MicroTimer MicroTimer50;  // timer 50hz
        static MicroTimer MicroTimer100; // timer 100hz

        public static void StartTimerServices() 
        {
            MicroStopwatch.Start();

            MicroTimer50 = new MicroTimer();
            MicroTimer50.Interval = 20000; // In nanoseconds
            MicroTimer50.MicroTimerElapsed += handleTimer50;
            // Can choose to ignore event if late by Xµs (by default will try to catch up)
            // microTimer.IgnoreEventIfLateBy = 500; // 500µs (0.5ms)
            MicroTimer50.Enabled = true; // Start timer


            MicroTimer100 = new MicroTimer();
            MicroTimer100.Interval = 10000; // In nanoseconds
            MicroTimer100.MicroTimerElapsed += handleTimer100;
            // Can choose to ignore event if late by Xµs (by default will try to catch up)
            // microTimer.IgnoreEventIfLateBy = 500; // 500µs (0.5ms)
            MicroTimer100.Enabled = true; // Start timer

        }

        public static void handleTimer50(object sender,
                                  GraphismVAS0.Services.MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            lock (mtxHndl50)
            {
                try
                {
                    foreach (var v in mHandlers50)
                        v.onEvent();
                }
                catch
                {
                }
            }
        }

        public static void handleTimer100(object sender,
                                  GraphismVAS0.Services.MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            lock (mtxHndl100)
            {
                try
                {
                    foreach (var v in mHandlers100)
                        v.onEvent();
                }
                catch
                {
                }
            }
        }


        public static void registerInTimer50(IEventHandlerTimer hndl)
        {
            lock (mtxHndl50)
            {
                mHandlers50.Add(hndl);
            }
        }

        public static void registerInTimer100(IEventHandlerTimer hndl)
        {
            lock (mtxHndl100)
            {
                mHandlers100.Add(hndl);
            }
        }

        public static void unregisterInTimer50(IEventHandlerTimer hndl)
        {
            lock (mtxHndl50)
            {
                mHandlers50.Remove(hndl);
            }
        }

        public static void unregisterInTimer100(IEventHandlerTimer hndl)
        {
            lock (mtxHndl100)
            {
                mHandlers100.Remove(hndl);
            }
        }


    }

    public class GraphismVAS2015_0 : IGraphism
    {
        static readonly object mtxSendDelay = new object();
        static private uint _sendDataDelay;  // Protegido por mtxSendDelay
        static public uint SendDataDelay { 
            get 
            {
                lock(mtxSendDelay) 
                {
                    return _sendDataDelay;
                }
            } 
            set
            {
                lock (mtxSendDelay)
                {
                    _sendDataDelay = value;
                }
            }
        }




        public IInfoActionsGeneratorFactory OutputControllerFactory { get; protected set; }
        public IInfoStateControllerFactory OutputServicesControllerFactory { get; protected set; }
        public IGraphicControllerOutputFactory GraphicControllerOutputFactory { get; protected set; }

        // ------------------------------------------------------------------------------------------------------------
        public GraphismVAS2015_0( IRenderEngine renderEngine, IComputerVisionManager cvManager  )
        {
            OutputControllerFactory = new InfoActionsGeneratorFactoryVAS0();    // Generadores de acciones
            OutputServicesControllerFactory = new InfoStateControllerFactoryVAS0( renderEngine, cvManager ); // Controlador de servicios adicionales
            GraphicControllerOutputFactory = new GraphicControllerOutputFactoryVAS0();  // Generadores de animaciones

            TimerServices.StartTimerServices();

        }



    }
}
