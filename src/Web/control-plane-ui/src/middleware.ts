import { NextRequest, NextResponse } from "next/server";

export const config = {
    matcher: ["/api/:path*", "/hls/:path*"],
};

export function middleware(request: NextRequest) {
    const { pathname, search } = request.nextUrl;

    if (pathname.startsWith("/hls/")) {
        const newPath = pathname.replace(/^\/hls\//, '');
        return NextResponse.rewrite(
            `${process.env.STREAMER_URL}/${newPath}${search}`
        );
    }

    return NextResponse.rewrite(
        `${process.env.API_URL}/${pathname}${search}`,
        { request }
    );
}