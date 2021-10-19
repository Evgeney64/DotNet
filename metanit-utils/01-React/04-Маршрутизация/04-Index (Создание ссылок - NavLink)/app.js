const http = require("http");
const fs = require("fs");
   
http.createServer(function(request, response){
       
    fs.readFile("index.html", function(error, data){   
        response.end(data);
    });
     
}).listen(3000, function(){
    console.log("http://localhost:3000");
    console.log("http://localhost:3000/about");
    console.log("http://localhost:3000/products");
    console.log("http://localhost:3000/products/ddd");
});