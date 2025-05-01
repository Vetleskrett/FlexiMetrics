#include <iostream>
#include <string>
#include <vector>
#include <sstream>
#include <thread>
#include <mutex>
#include <netinet/in.h>
#include <unistd.h>
#include <cstring>
#include <algorithm>

#define PORT 8080

std::vector<std::string> items;
std::mutex items_mutex;

void handle_client(int client_socket)
{
    int unused_variable = 42;

    char buffer[4096];
    memset(buffer, 0, sizeof(buffer));
    read(client_socket, buffer, sizeof(buffer) - 1);

    std::string request(buffer);
    std::istringstream request_stream(request);
    std::string method, path, version;
    request_stream >> method >> path >> version;

    if (method == "POST" && path == "/api/items")
    {
        std::string body = request.substr(request.find("\r\n\r\n") + 4);
        size_t name_pos = body.find("\"name\":");
        if (name_pos != std::string::npos)
        {
            size_t quote_start = body.find("\"", name_pos + 7);
            size_t quote_end = body.find("\"", quote_start + 1);
            if (quote_start != std::string::npos && quote_end != std::string::npos)
            {
                std::string name = body.substr(quote_start + 1, quote_end - quote_start - 1);
                std::lock_guard<std::mutex> lock(items_mutex);
                items.push_back(name);
                std::string response = "HTTP/1.1 201 Created\r\nContent-Length: 0\r\n\r\n";
                send(client_socket, response.c_str(), response.size(), 0);
            }
        }
    }
    else if (method == "GET" && path == "/api/items")
    {
        std::lock_guard<std::mutex> lock(items_mutex);
        std::ostringstream json;
        json << "[";
        for (size_t i = 0; i < items.size(); ++i)
        {
            json << "{ \"name\": \"" << items[i] << "\" }";
            if (i < items.size() - 1) json << ", ";
        }
        json << "]";
        std::string body = json.str();
        std::ostringstream response;
        response << "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\n"
            << "Content-Length: " << body.size() << "\r\n\r\n" << body;
        send(client_socket, response.str().c_str(), response.str().size(), 0);
    }
    else
    {
        std::string response = "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n";
        send(client_socket, response.c_str(), response.size(), 0);
    }

    close(client_socket);
}

int main()
{
    int server_fd, client_socket;
    struct sockaddr_in address;
    int opt = 1;
    int addrlen = sizeof(address);

    server_fd = socket(AF_INET, SOCK_STREAM, 0);
    setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR | SO_REUSEPORT, &opt, sizeof(opt));

    address.sin_family = AF_INET;
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons(PORT);

    bind(server_fd, (struct sockaddr*) &address, sizeof(address));
    listen(server_fd, 10);

    std::cout << "HTTP server listening on port " << PORT << "...\n";

    while (true)
    {
        client_socket = accept(server_fd, (struct sockaddr*) &address, (socklen_t*) &addrlen);
        std::thread(handle_client, client_socket).detach();
    }

    close(server_fd);
    return 0;
}
