class Order {
    items = [];
    totalAmount = 0;
    discountAmout = 0;
    amount = 0;
    customer = {};

    initialize() {
        this.customer = {
            name: "",
            phoneNumber: "",
            address: ""
        };
        this.items = [];
        this.amount = 0;
        this.discountAmout = 0;
        this.totalAmount = 0;
    }

    getOrderPrice() {
        return {
            amount: this.amount,
            discountAmount: this.discountAmout,
            totalAmount: this.totalAmount
        }
    }

    reCalculateOrderPrice() {
        let tempAmount = 0;
        for(let i = 0; i < this.items.length; i++) {
            tempAmount += (this.items[i].quantity * this.items[i].price);
        }

        this.amount = tempAmount;

        this.totalAmount = this.amount - this.discountAmout;


        return this.getOrderPrice();
    }

    addItem(id, quantity, price) {
        const item = this.items.find(e => e.id === id);
        if (item == null || item == undefined) {
            this.items.push({
                id,
                quantity,
                price
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

        return true;
    }

    removeItem(id) {
        this.items = this.items.filter(e => e.id !== id);
        this.reCalculateOrderPrice();
    }
}