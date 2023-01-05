async function LoadMySubPages(page = 1) {
    $('#paginationContainer').hide()
    let userId = $('#userId').attr('value')
    $('#mySubPagesContainer')
        .html('<div class="card"><div class="card-skeleton description"><div></div>')
/*        .html('<div class="lds-roller"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div>')*/
    await $('#mySubPagesContainer').load(`/SubPages/MySubPagesList?userId=${userId}&pageNumber=${page}`,function () {
        $('#paginationContainer').fadeToggle()
    })
    
}


let paginationConatainers = document.querySelectorAll('.pagination-container')
let paginationButtons = document.querySelectorAll('.pagination-item')

if (paginationConatainers.length != 0) {
    paginationConatainers[0].className += " active-container"
    paginationButtons[0].className += " active"
}

let nextItems = document.querySelectorAll('.next-item')
let previousItems = document.querySelectorAll('.previous-item')

if (nextItems.length != 0) {

    $(nextItems[nextItems.length - 1]).removeClass('next-item')
    $(nextItems[nextItems.length - 1]).hide()

    for (let i = 0; i < nextItems.length; ++i) {
        nextItems[i].addEventListener('click', function (e) {
            let activePaginationContainer = document.querySelector('.active-container')

            let activeItem = activePaginationContainer.querySelector('.pagination-item.active')
           
            let nextPaginationContainer = activePaginationContainer.nextElementSibling
            if (nextPaginationContainer != null) {

                $(activePaginationContainer).removeClass('active-container')
                $(activePaginationContainer).addClass('hide-container')
                $(activeItem).removeClass('active')

                $(nextPaginationContainer).addClass('active-container')
                let firstPaginationItem = nextPaginationContainer.querySelector('.pagination-item')
                let num = Number.parseInt(firstPaginationItem.textContent)
                LoadMySubPages(num)
                $(firstPaginationItem).addClass('active')
            }
        })


    }
}
if (previousItems.length != 0) {

    $(previousItems[0]).removeClass('previous-item')
    $(previousItems[0]).hide()
    for (let i = 0; i < previousItems.length; ++i) {

        previousItems[i].addEventListener('click', function () {
            let activePaginationContainer = document.querySelector('.active-container')

            let activeItem = activePaginationContainer.querySelector('.pagination-item.active')
            let previousPaginationContainer = activePaginationContainer.previousElementSibling
            if (previousPaginationContainer != null) {

                $(activePaginationContainer).removeClass('active-container')
                $(activePaginationContainer).addClass('hide-container')
                $(activeItem).removeClass('active')

                $(previousPaginationContainer).addClass('active-container')
                let firstPaginationItem = previousPaginationContainer.querySelector('.pagination-item')
                let num = Number.parseInt(firstPaginationItem.textContent)
                LoadMySubPages(num)
                $(firstPaginationItem).addClass('active')
            }

        })
    }
}

if (paginationButtons.length!=0) {
   
    LoadMySubPages()
    for (let i = 0; i < paginationButtons.length; ++i) {
        paginationButtons[i].addEventListener('click', (e) => {
            $('.pagination-item.active').removeClass('active')
            LoadMySubPages(e.target.textContent)
            $(e.target).addClass('active')
        })
    }
}
else {
    LoadMySubPages()
}