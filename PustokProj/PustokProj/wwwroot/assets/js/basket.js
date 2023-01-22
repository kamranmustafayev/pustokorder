let addBasketBtns = document.querySelectorAll(".btn-addbasket")
let delBtns = document.querySelectorAll(".btn-deletebook")
delBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault()
    fetch(btn.getAttribute("href")).then(response => response.json()).then(response => {
        Toastify({
            text: "Deleted from shopping cart",
            gravity: "bottom",
            duration: 3000,
            style: {
                background: "red"
            }
        }).showToast();
        console.log(response)
        if (response.bookCount == 0) btn.parentElement.parentElement.remove()
        else {
            btn.parentElement.querySelector("#bookCount").innerHTML = response.bookCount
            btn.parentElement.querySelector("#bookPrice").innerHTML = (response.bookPrice*response.bookCount)
        }
        document.getElementById("cartTotal").innerHTML = response.total
        document.getElementById("cartCount").innerHTML = response.count
    })
}))

addBasketBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();
    url = btn.getAttribute("href")
    fetch(url).then(res => {
        if (res.status == 200) {
            Toastify({
                text: "Added to shopping cart",
                gravity: "bottom",
                duration: 3000,
                style: {
                    background: "green"
                    }
            }).showToast();
            fetch("/Cart/ShowBlock").then(response => {
                return response.text()
            }).then(function (html) {
                block = document.querySelector(".cart-block")
                block.innerHTML = html
                block.querySelectorAll(".btn-deletebook").forEach(btn => btn.addEventListener("click", function (e) {
                    e.preventDefault()
                    fetch(btn.getAttribute("href")).then(response => response.json()).then(response => {
                        Toastify({
                            text: "Deleted from shopping cart",
                            gravity: "bottom",
                            duration: 3000,
                            style: {
                                background: "red"
                            }
                        }).showToast();
                        console.log(response)
                        if (response.bookCount == 0) btn.parentElement.parentElement.remove()
                        else {
                            btn.parentElement.querySelector("#bookCount").innerHTML = response.bookCount
                            btn.parentElement.querySelector("#bookPrice").innerHTML = (response.bookPrice * response.bookCount)
                        }
                        document.getElementById("cartTotal").innerHTML = response.total
                        document.getElementById("cartCount").innerHTML = response.count
                    })
                }))
            })
        }
        else alert("Unknown error")
    })
}))