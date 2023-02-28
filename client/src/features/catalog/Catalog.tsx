import { useEffect, useState } from 'react';
import ProductList from './ProductList';
import agent from '../../app/api/agent';
import { Product } from '../../app/models/product';
import LoadingComponent from '../../app/layout/LoadingComponent';

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
