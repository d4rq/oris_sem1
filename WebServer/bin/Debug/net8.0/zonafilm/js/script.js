if (window.location.href.at(-1) === "/") window.onload = () => load_movies();
if (window.location.href.split('/').at(-1).split('?').at(0) === "movie") window.onload = () => fix_date();

async function load_movies(){
    let cards = document.getElementById("cards");
    let response = await fetch("/movies");
    let data = await response.json();
    
    data.movies.forEach(data => {
       let newMovie = document.createElement("a");
       
       let image = document.createElement("img");
       image.src = data.Preview;
       image.classList.add("card-image");
       newMovie.classList.add("card");
       newMovie.append(image);
       
       let info = document.createElement("div");
       info.classList.add("card-info");
       let year = document.createElement("div");
       year.innerHTML = data.Year;
       let rating = document.createElement("div");
       rating.innerHTML = data.Rating;
       info.appendChild(year);
       info.appendChild(rating);
       newMovie.appendChild(info);
       
       let name = document.createElement("div");
       name.classList.add("card-name");
       name.innerHTML = data.Name;
       newMovie.appendChild(name);
       newMovie.href = "movie?id=" + data.Id;
       
       cards.appendChild(newMovie);
    });
}

function fix_date(){
    let date = document.getElementById("date");
    date.innerHTML = date.innerHTML.split(' ').at(0);
}