using PhysX;
using PhysX.VisualDebugger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class ControllerHitReport : UserControllerHitReport
    {
        public override void OnControllerHit(ControllersHit hit)
        {
            Console.WriteLine("Hit controller at " + hit.WorldPosition);
        }

        public override void OnObstacleHit(ControllerObstacleHit hit)
        {

        }

        public override void OnShapeHit(ControllerShapeHit hit)
        {

        }
    }

    public class ErrorOutput : ErrorCallback
    {
        public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
        {
            Console.WriteLine("PhysX: " + message);
        }
    }

    public class SampleFilterShader : SimulationFilterShader
    {
        public override FilterResult Filter(int attributes0, FilterData filterData0, int attributes1, FilterData filterData1)
        {
            return new FilterResult
            {
                FilterFlag = FilterFlag.Default,
                // Cause PhysX to report any contact of two shapes as a touch and call SimulationEventCallback.OnContact
                PairFlags = PairFlag.ContactDefault | PairFlag.NotifyTouchFound | PairFlag.NotifyTouchLost
            };
        }
    }

    public class PhysXTestEngine
    {
        public float StartTime;
        static void Main(string[] Args)
        {
            PhysXTestEngine pse = new PhysXTestEngine();
            var frameTimer = Stopwatch.StartNew();
            pse.Awake();

            while (true)
            {
                if (frameTimer.Elapsed >= TimeSpan.FromMilliseconds(16.67))
                {
                    pse.Update(frameTimer.Elapsed);

                    frameTimer.Restart();
                }
                
            }

            Console.Read();
        }


        public Physics Physics { get; private set; }
        public Scene Scene { get; private set; }



        public void Awake()
        {
            Console.WriteLine("StartGame");
            //初始化物理
            ErrorOutput errorOutput = new ErrorOutput();
            Foundation foundation = new Foundation(errorOutput);

            //var pvd = new PhysX.VisualDebugger.ConnectionManager
            Physics = new Physics(foundation);
            var sceneDesc = new SceneDesc
            {
                Gravity = new System.Numerics.Vector3(0, -9.81f, 0),
                FilterShader = new SampleFilterShader()
            };
            Scene = Physics.CreateScene(sceneDesc);

            this.Scene.SetVisualizationParameter(VisualizationParameter.Scale, 2.0f);
            this.Scene.SetVisualizationParameter(VisualizationParameter.CollisionShapes, true);
            this.Scene.SetVisualizationParameter(VisualizationParameter.JointLocalFrames, true);
            this.Scene.SetVisualizationParameter(VisualizationParameter.JointLimits, true);
            this.Scene.SetVisualizationParameter(VisualizationParameter.ActorAxes, true);

            Physics.PvdConnectionManager.Connect("localhost", 5425);

            //创建平面
            var groundPlaneMaterial = this.Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
            var groundPlane = this.Scene.Physics.CreateRigidStatic();
            groundPlane.GlobalPose = Matrix4x4.CreateFromAxisAngle(new System.Numerics.Vector3(0, 0, 1), (float)System.Math.PI / 2);
            groundPlane.Name = "Ground Plane";
            var planeGeom = new PlaneGeometry();
            groundPlane.CreateShape(planeGeom, groundPlaneMaterial);
            this.Scene.AddActor(groundPlane);

            //创建角色
            var material = Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
            var controllerManager = Scene.CreateControllerManager();
            {
                var desc = new CapsuleControllerDesc()
                {
                    Height = 4,
                    Radius = 1,
                    Material = material,
                    UpDirection = new Vector3(0, 1, 0),
                    Position = new Vector3(0, 3, 0),
                    ReportCallback = new ControllerHitReport()
                };

                _controller = controllerManager.CreateController<CapsuleController>(desc);
            }
            //再来一个不动的
            {
                var desc = new CapsuleControllerDesc()
                {
                    Height = 4,
                    Radius = 1,
                    Material = material,
                    UpDirection = new Vector3(0, 1, 0),
                    Position = new Vector3(15, 3, 15)
                };
                
                controllerManager.CreateController<CapsuleController>(desc);
            }
        }

        private CapsuleController _controller;

        public void Update(TimeSpan elapsed)
        {
            Scene.Simulate((float)elapsed.TotalSeconds);
            Scene.FetchResults(true);
            _controller.Move(new Vector3(1, 0, 1), elapsed);
            Console.WriteLine(_controller.Position);
        }

    }
}
