import { useState, useEffect } from 'react';
import { Product } from '../../models/product';
import ProductList from './ProductList';
import LoadingComponent from '../../../layout/LoadingComponent';
import agent from '../../api/agent';

export default function Catalog() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async function getProduct() {
      try {
        const products = await agent.Catalog.list();
        setProducts(products);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    })();
  }, []);
  if (loading) return <LoadingComponent message="Loading products..." />;

  return (
    <>
      <ProductList products={products} />
    </>
  );
}
