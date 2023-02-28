import { createBrowserRouter, Navigate } from 'react-router-dom';
import App from '../layout/App';

import NotFound from '../errors/NotFound';
import ServerError from '../errors/ServerError';
import HomePage from '../../features/home/HomePage';
import BasketPage from '../../features/basket/BasketPage';
import ProductDetails from '../../features/catalog/ProductDetails';
import AboutPage from '../../features/about/AboutPage';
import CheckoutPage from '../../features/checkout/CheckoutPage';
import Catalog from '../../features/catalog/Catalog';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      { path: '', element: <HomePage /> },
      { path: 'catalog', element: <Catalog /> },
      { path: 'catalog/:id', element: <ProductDetails /> },
      { path: 'about', element: <AboutPage /> },
      { path: 'contact', element: <HomePage /> },
      { path: 'server-error', element: <ServerError /> },
      { path: 'not-found', element: <NotFound /> },
      { path: 'basket', element: <BasketPage /> },
      { path: 'checkout', element: <CheckoutPage /> },
      { path: '*', element: <Navigate replace to="/not-found" /> }
    ]
  }
]);
