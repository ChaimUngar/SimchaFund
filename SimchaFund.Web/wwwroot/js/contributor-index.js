$(() => {

    const newContributor = new bootstrap.Modal($(".new-contrib")[0]);
    const deposit = new bootstrap.Modal($(".deposit")[0]);
    const edit = new bootstrap.Modal($(".edit-contrib")[0]);


    $("#new-contributor").on('click', function () {
        newContributor.show()
    }) //works

    $(".cancel-contrib").on('click', function () {
        newContributor.hide()
    }) //works

    $("#search").on('input', function () {
        const searchText = $("#search").val()
        console.log(searchText)
        search(searchText)
    }) //x works 

    function search(text) {
        $(".rows").each(function () {
            const row = $(this)
            const name = row.find("#name")

            
         })
    } //.contains

    $("#clear").on('click', function () {
        $("#search").val("")
       search("")
    }) //works

    $(".table-responsive").on('click', '.deposit-button', function () {
        //console.log("hello")
        const tr = $(this).closest('tr')
        //console.log('hello')
        console.log(tr.data('person-id'))
        deposit.show()
    }) //problem is data attr

    //$("table").on('click', '.btn-success', function () {
    //    const tr = $(this).closest('tr');
    //    console.log(tr.data('person-id'));
    //});

    $(".table-responsive").on('click', '.deposit-button', function () {
        const tr = $(this).closest('tr')
        //console.log(tr.data('first-name'))
        const name = tr.data('first-name')
        $("#deposit-name").text(name)
    }) //prob is data attr

    $(".cancel-deposit").on('click', function () {
        deposit.hide()
    }) //works

    $(".table-responsive").on('click', '.edit-button', function () {
        edit.show()

        const btn = $(this).closest('button')

        const first = btn.data('first-name')
        const last = btn.data('last-name')
        const cell = btn.data('cell')
        const always = btn.data('always-include')
        const date = btn.data('date')
        const id = btn.data('id')

        console.log(first)
        console.log(last)
        console.log(cell)
        console.log(always)
        console.log(date)
        console.log(id)

        $("#contributor_first_name").val(first)
        $("#contributor_last_name").val(last)
        $("#contributor_cell_number").val(cell)
        $("#contributor_created_at").val(date)
        $("#contributor_always_include").val(always)
        $("#edit-id").text(id)
        console.log('shld be showing edit...')
    }) //problem is data attr

    $(".cancel-edit").on('click', function () {
        edit.hide()
    }) //works

    //const tr = $(this).closest('tr');
    //console.log(tr.data('person-id'));
    
})