#!/bin/bash
echo "Creating hls folder"
mkdir /var/www/hls

echo "Starting nginx"
# Start nginx to serve HLS content
nginx &

echo "Starting rtsp converter"
./app/RtspConverter

