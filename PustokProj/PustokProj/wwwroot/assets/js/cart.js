let removeBtns = document.querySelectorAll(".removeitem-btn")
let delBtns = document.querySelectorAll(".btn-deletebook")
removeBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault()
    btn.parentElement.parentElement.remove()
}))

delBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault()
    fetch(btn.getAttribute("href")).then(res => {
        if (res.status == 200) location.reload()
    })
}))