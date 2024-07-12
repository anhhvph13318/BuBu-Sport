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
        if(this.items.length === 0) return this.getOrderPrice();

        this.amount = this.items.reduce((prev, current) => prev.price * prev.quantity + current.price * current.quantity, {
            price: 0,
            quantity: 0,
            productId: ''
        });
        this.totalAmount = this.amount - this.discountAmout;

        console.log(this.amount);

        return this.getOrderPrice();
    }

    addItem(productId, quantity, price) {
        const item = this.items.find(e => e.productId === productId);
        if (item == null || item == undefined) {
            this.items.push({
                productId,
                quantity,
                price
            });

            this.reCalculateOrderPrice();
            return;
        }

        items.quantity++;
        this.reCalculateOrderPrice();
    }

    updateQuantity(productId, quantity) {
        const item = this.items.find(e => e.productId === productId);
        if (item == null) return false;

        item.quantity = quantity;

        return true;
    }

    removeItem(productId) {
        this.items = this.items.filter(e => e.productId !== productId);
        this.reCalculateOrderPrice();
    }

    createPayload() {
        return {

        }
    }


}