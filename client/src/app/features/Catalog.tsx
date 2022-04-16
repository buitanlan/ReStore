import { useState, useEffect } from "react";
import {Product} from "../models/product";
import ProductList from "./ProductList";
import axios from "axios";

export default function Catalog() {
    const [products, setProducts] = useState<Product[]>([]);

    useEffect(() => {
        (async function getProduct(){
            const products = await axios.get<Product[]>('https://localhost:5000/api/products');
            setProducts(products.data)
        })()
    }, [])

    return (
        <>
            <ProductList products={products} />
        </>
    )
}
