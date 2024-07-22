class Order {
    items = [];
    totalAmount = 0;
    discountAmount = 0;
    amount = 0;
    customer = {
        id: ''
    };
    errors = [];

    getOrderPrice() {
        return {
            amount: this.amount,
            discountAmount: this.discountAmount,
            totalAmount: this.totalAmount
        }
    }

    reCalculateOrderPrice() {
        let tempAmount = 0;
        for(let i = 0; i < this.items.length; i++) {
            tempAmount += (this.items[i].quantity * this.items[i].price);
        }

        this.amount = tempAmount;

        this.totalAmount = this.amount - this.discountAmount;


        return this.getOrderPrice();
    }

    addItem(id, quantity, price, name, image) {
        const item = this.items.find(e => e.id === id);
        if (item == null || item == undefined) {
            this.items.push({
                id,
                quantity,
                price,
                name,
                image
            });

            this.reCalculateOrderPrice();
            return false;
        }

        item.quantity++;
        this.reCalculateOrderPrice();
        return true;
    }

    updateQuantity(id, quantity) {
        const item = this.items.find(e => e.id === id);
        if (item == null) return false;

        item.quantity = quantity;

        this.reCalculateOrderPrice();

        return true;
    }

    removeItem(id) {
        this.items = this.items.filter(e => e.id !== id) || [];
        this.reCalculateOrderPrice();
    }

    updateCustomerInfo(e) {
        this.customer = {
            ...this.customer,
            [e.target.name]: e.target.value
        }
    }

    pushError(key, value) {
        this.errors = [...this.errors, { key: value }]
    }

    dropError(key) {
        this.errors = this.errors.filter(e => e.key !== key);
    }

    verify() {
        return this.errors.length === 0;
    }
}