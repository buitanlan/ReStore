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
import RequireAuth from './RequiredAuthen';
import Register from '../../features/account/Register';
import Login from '../../features/account/Login';
import Orders from '../../features/orders/Orders';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        element: <RequireAuth />,
        children: [
          { path: 'checkout', element: <CheckoutPage /> },
          { path: 'orders', element: <Orders /> }
        ]
      },
      { path: '', element: <HomePage /> },
      { path: 'catalog', element: <Catalog /> },
      { path: 'catalog/:id', element: <ProductDetails /> },
      { path: 'about', element: <AboutPage /> },
      { path: 'contact', element: <HomePage /> },
      { path: 'server-error', element: <ServerError /> },
      { path: 'not-found', element: <NotFound /> },
      { path: 'basket', element: <BasketPage /> },
      { path: 'login', element: <Login /> },
      { path: 'register', element: <Register /> },
      { path: '*', element: <Navigate replace to="/not-found" /> }
    ]
  }
]);
