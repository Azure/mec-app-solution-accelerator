using MongoDB.Driver;
using ControlPlane.API.Models;
using Microsoft.Extensions.Options;
using ControlPlane.API.Settings;

namespace ControlPlane.API.Services
{
    public class CameraService
    {
        private readonly IMongoCollection<Camera> _cameras;

        public CameraService(MongoClient client, IOptions<MongoDBSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _cameras = database.GetCollection<Camera>("Cameras");
        }

        public List<Camera> GetAllCameras() =>
            _cameras.Find(camera => true).ToList();

        public Camera GetCameraById(string id) =>
            _cameras.Find<Camera>(camera => camera.Id == id).FirstOrDefault();

        public Camera AddCamera(Camera camera)
        {
            _cameras.InsertOne(camera);
            return camera;
        }

        public void UpdateCamera(string id, Camera updatedCamera) =>
            _cameras.ReplaceOne(camera => camera.Id == id, updatedCamera);

        public void DeleteCamera(string id) =>
            _cameras.DeleteOne(camera => camera.Id == id);
    }
}