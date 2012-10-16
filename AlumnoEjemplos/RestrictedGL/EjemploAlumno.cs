using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Example;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL
{
    struct Shared
    {
        public const string NombreGrupo = "RestrictedGL";
        public static readonly string MediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir + NombreGrupo + "\\";
        public static float ElapsedTime = 0;
    }

    public class EjemploAlumno : TgcExample
    {
        private Terrain.Terrain terrain;
        private Tank.Tank tank;

        private const float SCALE_Y = 5f;
        private const float SCALE_XZ = 50f;

        private readonly UserVars userVars;
        
        public EjemploAlumno() {
            this.userVars = new UserVars();
        }

        #region Descripciones
            /// <summary>
            /// Categor�a a la que pertenece el ejemplo.
            /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
            /// </summary>
            public override string getCategory() {
                return "AlumnoEjemplos";
            }

            /// <summary>
            /// Completar nombre del grupo en formato Grupo NN
            /// </summary>
            public override string getName() {
                return Shared.NombreGrupo;
            }

            /// <summary>
            /// Completar con la descripci�n del TP
            /// </summary>
            public override string getDescription() {
                return "SuperMegaTanque";
            }
        #endregion

        private void updateUserVars() {
            this.userVars
                .set("posX", GuiController.Instance.FpsCamera.Position.X)
                .set("posY", GuiController.Instance.FpsCamera.Position.Y)
                .set("posZ", GuiController.Instance.FpsCamera.Position.Z)
                .set("viewX", GuiController.Instance.FpsCamera.LookAt.X)
                .set("viewY", GuiController.Instance.FpsCamera.LookAt.Y)
                .set("viewZ", GuiController.Instance.FpsCamera.LookAt.Z);
        }

        /// <summary>C�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.</summary>
        public override void init() {
            var d3dDevice = GuiController.Instance.D3dDevice;

            this.terrain = new Terrain.Terrain(SCALE_XZ, SCALE_Y);

            var tankY = this.terrain.heightmapData[64, 64] * SCALE_Y;
            this.tank = new Tank.Tank(new Vector3(0, tankY + 15, 0),this.terrain);

            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 100f;
            GuiController.Instance.FpsCamera.JumpSpeed = 100f;
            GuiController.Instance.FpsCamera.setCamera(this.tank.Position + new Vector3(0, 200, 400), this.tank.Position);

            GuiController.Instance.Modifiers.addFloat("Cam Velocity", 0f, 1000f, 500f);
            GuiController.Instance.Modifiers.addFloat("tankVelocity", 0f, 1000f, 100f);

            this.userVars.addMany(
                "posX", 
                "posY",
                "posZ",
                "viewX",
                "viewY",
                "viewZ"
            );

            //Aumentar distancia del far plane
            d3dDevice.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                (float)d3dDevice.CreationParameters.FocusWindow.Width / d3dDevice.CreationParameters.FocusWindow.Height, 1f, 30000f);
        }

        ///<summary>Se llama cada vez que hay que refrescar la pantalla</summary>
        ///<param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime) {
            Shared.ElapsedTime = elapsedTime;

            if (GuiController.Instance.D3dInput.keyDown(Key.R)) {
                //R = C�mara en el origen, m�s o menos
                GuiController.Instance.FpsCamera.setCamera(new Vector3(-100, 200, 0), new Vector3(490f, 128f, -10f));
            }
            if (GuiController.Instance.D3dInput.keyDown(Key.T)) {
                //T = Deformar parte del terreno actual
                terrain.deform(GuiController.Instance.FpsCamera.Position.X, GuiController.Instance.FpsCamera.Position.Z, 150, 1);
            }
            
            GuiController.Instance.FpsCamera.MovementSpeed = 
            GuiController.Instance.FpsCamera.JumpSpeed =
                Modifiers.get<float>("Cam Velocity");
            
            this.updateUserVars();

            this.tank.render();
            this.terrain.render();
        }

        ///<summary>Se llama al cerrar la app. Hacer dispose() de todos los objetos creados</summary>
        public override void close() {
            this.terrain.dispose();
        }

    }
}