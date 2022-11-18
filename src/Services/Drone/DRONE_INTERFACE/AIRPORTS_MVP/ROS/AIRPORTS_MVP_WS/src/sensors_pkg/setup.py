from setuptools import setup

package_name = 'sensors_pkg'

setup(
    name=package_name,
    version='0.0.0',
    packages=[package_name],
    data_files=[
        ('share/ament_index/resource_index/packages',
            ['resource/' + package_name]),
        ('share/' + package_name, ['package.xml']),
    ],
    install_requires=['setuptools'],
    zip_safe=True,
    maintainer='root',
    maintainer_email='javier.carrera@turingchallenge.com',
    description='TODO: Package description',
    license='TODO: License declaration',
    tests_require=['pytest'],
    entry_points={
        'console_scripts': [
            
            'position_publisher = sensors_pkg.position_publisher:main',
            'range_sensor_publisher = sensors_pkg.range_sensor_publisher:main',
            'camera_publisher = sensors_pkg.camera_publisher:main',
            'sensor_data_subscriber = sensors_pkg.sensor_data_subscriber:main',
            
        ],
    },
)
