export const dynamic = 'force-dynamic';
export async function GET(_: Request) {
    return Response.json({
        API_URL: process.env.API_URL
    });
}