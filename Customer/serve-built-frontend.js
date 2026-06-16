const fs = require('fs');
const http = require('http');
const path = require('path');

const port = 4200;
const host = '127.0.0.1';
const root = path.join(__dirname, 'dist', 'Customer', 'browser');

const mimeTypes = {
  '.css': 'text/css',
  '.html': 'text/html',
  '.ico': 'image/x-icon',
  '.js': 'text/javascript',
  '.json': 'application/json',
};

function sendFile(response, filePath) {
  fs.readFile(filePath, (error, content) => {
    if (error) {
      response.writeHead(500);
      response.end('Unable to load frontend file.');
      return;
    }

    response.writeHead(200, {
      'Content-Type': mimeTypes[path.extname(filePath)] || 'application/octet-stream',
    });
    response.end(content);
  });
}

const server = http.createServer((request, response) => {
  const urlPath = decodeURIComponent((request.url || '/').split('?')[0]);
  const requestedPath = urlPath === '/' ? 'index.html' : urlPath.slice(1);
  const filePath = path.join(root, requestedPath);

  if (!filePath.startsWith(root)) {
    response.writeHead(403);
    response.end('Forbidden');
    return;
  }

  fs.stat(filePath, (error, stat) => {
    if (!error && stat.isFile()) {
      sendFile(response, filePath);
      return;
    }

    sendFile(response, path.join(root, 'index.html'));
  });
});

server.listen(port, host, () => {
  console.log(`Frontend running at http://${host}:${port}/`);
});
