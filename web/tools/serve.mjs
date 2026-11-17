// Minimal static file server for local play/testing: node web/tools/serve.mjs [port]
import http from "http";
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const port = Number(process.argv[2]) || 8123;
const TYPES = {
    ".html": "text/html; charset=utf-8",
    ".js": "text/javascript; charset=utf-8",
    ".mjs": "text/javascript; charset=utf-8",
    ".json": "application/json",
    ".png": "image/png",
    ".jpg": "image/jpeg",
    ".mp3": "audio/mpeg",
    ".wav": "audio/wav",
    ".ttf": "font/ttf",
    ".otf": "font/otf",
};

http.createServer((req, res) => {
    const urlPath = decodeURIComponent(new URL(req.url, "http://x").pathname);
    let file = path.join(root, urlPath);
    if (!file.startsWith(root)) {
        res.writeHead(403).end();
        return;
    }
    if (fs.existsSync(file) && fs.statSync(file).isDirectory()) file = path.join(file, "index.html");
    fs.stat(file, (err, stat) => {
        if (err) {
            res.writeHead(404).end("Not found");
            return;
        }
        const type = TYPES[path.extname(file).toLowerCase()] || "application/octet-stream";
        const range = req.headers.range && /bytes=(\d*)-(\d*)/.exec(req.headers.range);
        if (range) {
            // Range support lets <audio> stream and seek the long music tracks.
            const start = range[1] ? Number(range[1]) : 0;
            const end = range[2] ? Number(range[2]) : stat.size - 1;
            res.writeHead(206, { "Content-Type": type, "Content-Range": `bytes ${start}-${end}/${stat.size}`, "Accept-Ranges": "bytes", "Content-Length": end - start + 1 });
            fs.createReadStream(file, { start, end }).pipe(res);
        } else {
            res.writeHead(200, { "Content-Type": type, "Content-Length": stat.size, "Accept-Ranges": "bytes", "Cache-Control": "no-cache" });
            fs.createReadStream(file).pipe(res);
        }
    });
}).listen(port, () => console.log(`Kami No Tabi: http://localhost:${port}`));
