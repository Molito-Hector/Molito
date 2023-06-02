import { ScrollRestoration, useLocation } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { useStore } from '../stores/store';
import { useEffect } from 'react';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../common/modals/ModalContainer';
import SubscriptionPage from '../../features/subsctiptions/SubscriptionPage';
import { observer } from 'mobx-react-lite';

function Subscription() {
  const { commonStore, userStore } = useStore();

  useEffect(() => {
    if (commonStore.token) {
      userStore.getUser().finally(() => commonStore.setAppLoaded());
    } else {
      commonStore.setAppLoaded();
    }
  }, [commonStore, userStore])

  if (!commonStore.appLoaded) return <LoadingComponent content='Loading app...' />

  return (
    <>
      <ScrollRestoration />
      <ModalContainer />
      <ToastContainer position='bottom-right' hideProgressBar theme='colored' />
      <SubscriptionPage />
    </>
  );
}

export default observer(Subscription);
