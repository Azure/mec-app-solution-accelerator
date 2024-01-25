
import type { NextApiRequest, NextApiResponse } from 'next';

type ResponseData = {
    API_URL: string | undefined;
};

export default function handler(
    req: NextApiRequest,
    res: NextApiResponse<ResponseData>
) {
    // Accessing an environment variable
    const apiUrl = process.env.API_URL;

    // Returning a JSON response
    res.status(200).json({ API_URL: apiUrl });
}