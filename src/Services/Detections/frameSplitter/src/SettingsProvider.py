import os

class SettingsProvider:
    def __init__(self):
        self.env_var_cache = {}

    def get_env_variables_with_prefix(self, prefix):
        if prefix in self.env_var_cache:
            return self.env_var_cache[prefix]

        for key, value in os.environ.items():
            if key.startswith(prefix):
                self.env_var_cache[prefix] = value
                return value

        return None

    def get_camera_id(self):
        camera_id = self.get_env_variables_with_prefix("CAMERA_ID_")
        return camera_id

    def get_rtsp_uri(self):
        rtsp = self.get_env_variables_with_prefix("RTSP_URI_")
        return rtsp
