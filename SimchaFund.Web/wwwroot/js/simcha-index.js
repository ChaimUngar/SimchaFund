$(() => {

    const newSimcha = new bootstrap.Modal($("#simcha-modal")[0]);

    $("#new-simcha").on('click', function () {
        newSimcha.show()
    })

    $("#cancel").on('click', function () {
        newSimcha.hide()
    })
})