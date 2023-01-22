let deleteBtns = document.querySelectorAll(".btn-delete-image")

deleteBtns.forEach(btn => btn.addEventListener("click", function () {
    btn.parentElement.parentElement.remove()
}))