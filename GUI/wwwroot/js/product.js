class ProductStorage {
    OldQuantity = 0;

    async filter(input) {
        try {
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

            // Ki?m tra n?u data ho?c lstProduct không t?n t?i
            if (!json?.data?.lstProduct || !Array.isArray(json.data.lstProduct)) {
                console.warn("Không có s?n ph?m tr? v? ho?c sai ??nh d?ng:", json);
                return []; // Tr? v? m?ng r?ng ?? tránh l?i
            }

            return json.data.lstProduct;
        } catch (error) {
            console.error("L?i khi g?i API filter:", error);
            return []; // Tránh throw l?i, tr? v? m?ng r?ng an toàn
        }
    }
}
