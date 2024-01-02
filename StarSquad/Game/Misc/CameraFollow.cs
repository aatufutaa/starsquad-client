using StarSquad.Lobby.UI.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StarSquad.Game.Misc
{
    public class CameraFollow
    {
        public Camera camera;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        private Vector3 vel = Vector3.zero;

        private float x;
        private float y;

        private float lastX;
        private float lastY;

        private Vector2 forward = Vector3.zero;

        private bool shaking;
        private int shakeTicks;

        private float cameraOffsetY;
        private float cameraOffsetZ;

        private float targetX;
        private float targetY;

        private bool zoomOut;
        private float zoomTimer;
        private float zoomFrom;
        private float zoomTo;

        public CameraFollow()
        {
        }

        public void Init(int mapSizeX, int mapSizeY)
        {
            this.camera = GameObject.Find("GameCamera").GetComponent<Camera>();
            AngleUtil.FixAngle(this.camera.transform, -20);
            // adjust camera size

            // min size 12 hor 30 ver
            const float minCameraWidth = 6f; // 12 blocks in x
            const float minCameraHeight = 10.5f;

            var aspect = this.camera.aspect;
            var cameraHeight = minCameraWidth * (1f / aspect);
            this.camera.orthographicSize = cameraHeight;
            if (this.camera.orthographicSize < minCameraHeight)
            {
                this.camera.orthographicSize = minCameraHeight;
            }
            
            this.cameraOffsetY = this.camera.transform.position.y;
            this.cameraOffsetZ = this.camera.transform.position.z;
            
            // camera x fix
            var cameraWidth = this.camera.orthographicSize / (1f / aspect);
            const float border = 5.5f; 
            var fixedSizeX = mapSizeX / 2;
            this.minX = -fixedSizeX + cameraWidth - border;
            this.maxX = fixedSizeX - cameraWidth + border;

            // camera y fix
            var pos = this.camera.transform.position;
            this.camera.transform.position = Vector3.zero;
            var ray = this.camera.ViewportPointToRay(new Vector3(0, 1, 0));
            var vec = ray.origin + (((ray.origin.y - 0f) / -ray.direction.y) * ray.direction);
            var offset = vec.z; // amount of blocks per half camera
            var fixedSizeY = mapSizeY / 2;
            this.minY = -fixedSizeY + offset - border;
            this.maxY = fixedSizeY - offset + border;
            this.camera.transform.position = pos;
        }

        public void Tick()
        {
            this.lastX = this.x;
            this.lastY = this.y;

            this.x = this.targetX;
            this.y = this.targetY;

            var dx = this.x - this.lastX;
            var dy = this.y - this.lastY;
            this.forward = new Vector2(dx, dy).normalized * 2f;

            if (this.shaking)
            {
                ++this.shakeTicks;
                if (this.shakeTicks > 5)
                {
                    this.shaking = false;
                }
            }
        }

        public void Render(float partialTicks)
        {
            var x = this.x;
            var y = this.y;

            x += this.forward.x;
            y += this.forward.y;

            this.camera.transform.position = Vector3.SmoothDamp(
                this.camera.transform.position,
                this.GetCameraPosition(x, y),
                ref vel,
                20f * Time.deltaTime
            );

            if (this.shaking)
            {
                const float mul = 0.1f;
                var x1 = Random.Range(-1f, 1f) * mul;
                var y1 = Random.Range(-1f, 1f) * mul;
                this.camera.transform.position += new Vector3(x1, y1);
            }

            if (this.zoomOut)
            {
                this.zoomTimer += Time.deltaTime;
                const float zoomOutTimer = 1.2f;
                var p = this.zoomTimer / zoomOutTimer;
                if (p > 1f)
                {
                    p = 1f;
                    this.zoomOut = false;
                }
                p = MathHelper.SmoothLerp(p);
                this.camera.orthographicSize = this.zoomFrom + (this.zoomTo - this.zoomFrom) * p;
            }
        }

        private Vector3 GetCameraPosition(float x, float y)
        {
            if (x < this.minX) x = this.minX;
            else if (x > this.maxX) x = this.maxX;

            if (y < this.minY) y = this.minY;
            else if (y > this.maxY) y = this.maxY;

            return new Vector3(x, this.cameraOffsetY, y + this.cameraOffsetZ);
        }


        public void SetTarget(float x, float y)
        {
            this.targetX = x;
            this.targetY = y;
        }

        public void SetTargetAndReset(float x, float y)
        {
            this.SetTarget(x, y);
            this.lastX = x;
            this.lastY = y;
            this.forward = Vector2.zero;
            this.camera.transform.position = this.GetCameraPosition(x, y);
        }

        public void Shake()
        {
            this.shaking = true;
            this.shakeTicks = 0;
        }

        public void ZoomOut()
        {
            this.zoomOut = true;
            this.zoomTimer = 0f;
            this.zoomFrom = this.camera.orthographicSize;
            this.zoomTo = this.zoomFrom + 1 * (1f / this.camera.aspect);
        }
    }
}