class ProductStorage {

    async filter(input) {
        const res = await fetch(PRODUCT_FILTER_API, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: input
            })
        });
        const json = await res.json();
        return json.data.lstProduct;
    }

    async getProduct(id) {
        const res = await fetch(`${PRODUCT_SELECT_API}/${id}`, {
            
        })
    }
}