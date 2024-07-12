class ProductStorage {
    products = []

    constructor() {
        this.initialize();
    }

    initialize() {
        this.products = [
            {
            id: 1,
            code: 'SP01',
            name: 'San pham 1',
            price: 100,
            },
            {
                id: 2,
                code: 'SP21',
                name: 'Ti vi 1',
                price: 150
            },
            {
                id: 3,
                code: 'SP02',
                name: 'San pham 1',
                price: 300
            }
        ];

        return;

        fetch('', {
            
        }).then(res => res.json())
        .then(res => this.products = res.data)
        .catch(err => {
            alert("Lỗi hệ thống");
            console.log(err);
        })
    }

    filter(input) {
        return this.products.filter(e => e.name.startsWith(input) || e.code.startsWith(input));
    }
}