class OrderStorage {
    orders = [];

    constructor() {
        this.addOrder(0);
    }

    getOrder(id) {
        return this.orders.find(e => e.tempId == id);
    }

    addOrder(orderNumber) {
        const order = new Order();
        order.tempId = orderNumber;

        this.orders.push(order);

        return order;
    }

    getNextOrderNumber() {
        return this.orders.length;
    }

    async createOrder(tempId) {
        const order = this.getOrder(tempId);
        const res = await fetch(ORDER_SUBMIT_API, {
            headers: {
                'Content-Type': 'application/json'
            },
            method: 'POST',
            body: JSON.stringify(order)
        });

        if (res.status === 204) {
            alert("Tạo thành công");
            this.orders = this.orders.filter(e => e.tempId != order.tempId);
            return true;
        }

        return false;
    }
}