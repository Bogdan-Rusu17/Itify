import {useCallback, useEffect, useRef, useState} from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import HomeIcon from '@mui/icons-material/Home';
import { Link } from 'react-router-dom';
import { AppRoute } from 'routes';
import { useIntl } from 'react-intl';
import { useAppDispatch, useAppSelector } from '@application/store';
import {Drawer, IconButton} from '@mui/material';
import { resetProfile } from '@application/state-slices';
import { useAppRouter } from '@infrastructure/hooks/useAppRouter';
import { NavbarLanguageSelector } from '@presentation/components/ui/NavbarLanguageSelector/NavbarLanguageSelector';
import { useOwnUserHasRole } from '@infrastructure/hooks/useOwnUser';
import { UserRoleEnum } from '@infrastructure/apis/client';
import {MenuSharp} from "@mui/icons-material";

/**
 * This is the navigation menu that will stay at the top of the page.
 */
export const Navbar = () => {
  const {formatMessage} = useIntl();
  const {loggedIn} = useAppSelector(x => x.profileReducer);
  const isAdmin = useOwnUserHasRole(UserRoleEnum.Admin);
  const dispatch = useAppDispatch();
  const {redirectToHome} = useAppRouter();
  const logout = useCallback(() => {
    dispatch(resetProfile());
    redirectToHome();
  }, [dispatch, redirectToHome]);
  const menuRef = useRef<HTMLDivElement | null>(null);
  const [height, setHeight] = useState<number>();
  const [toggleMenu, setToggleMenu] = useState(false);

  const toggleDrawer = (newOpen: boolean) => () => {
    setToggleMenu(newOpen);
  };

  useEffect(() => {
    if (!menuRef.current) return;

    const observer = new ResizeObserver((entries) => {
      for (const entry of entries) {
        setHeight(entry.contentRect.height);
      }
    });

    observer.observe(menuRef.current);

    return () => observer.disconnect();
  }, [menuRef]);

  const MenuList = () => (
      <div
          className="flex flex-col lg:flex-row lg:items-center text-white gap-2 bg-app-blue h-full pt-10 px-4 lg:pt-0 lg:px-0 min-w-[260px]">
        {isAdmin &&
            <> { /*If the user is logged in and it is an admin they can have new menu items shown.*/ }
              <div className="flex flex-col lg:flex-row items-center gap-2 border-b-2 border-white lg:border-none pb-4 lg:pb-0 hover:bg-app-dark-blue rounded-md">
                <Link
                    to={AppRoute.Users}
                    className="p-4 font-semibold text-center text-white"
                >
                  {formatMessage({id: "globals.users"})}
                </Link>
              </div>
              <div className="flex flex-col lg:flex-row items-center gap-2 border-b-2 border-white lg:border-none pb-4 lg:pb-0 hover:bg-app-dark-blue rounded-md">
                <Link
                    to={AppRoute.UserFiles}
                    className="p-4 font-semibold text-center text-white"
                >
                  {formatMessage({id: "globals.files"})}
                </Link>
              </div>
            </>
        }

        <div className="ml-auto p-4 flex flex-col lg:flex-row lg:items-center gap-5">
          <NavbarLanguageSelector/>
          {!loggedIn && <Button color="inherit">  {/* If the user is not logged in show a button that redirects to the login page. */}
            <Link className="text-white" to={AppRoute.Login}>
              {formatMessage({id: "globals.login"})}
            </Link>
          </Button>}
          {loggedIn && <Button onClick={logout} color="inherit"> {/* Otherwise show the logout button. */}
            {formatMessage({id: "globals.logout"})}
          </Button>}
        </div>
      </div>
  );

  return (
      <div className="w-full">
        <div className="w-full top-0 z-50 fixed bg-app-blue" ref={menuRef}>
          <div className="px-2 flex items-center text-white gap-2">
            <Link className="shrink-0 h-10 w-10"
                to={AppRoute.Index}> {/* Add a button to redirect to the home page. */}
              <IconButton>
                <HomeIcon style={{color: 'white'}} fontSize='large'/>
              </IconButton>
            </Link>

            <div className="px-5 w-full hidden lg:block">{MenuList()}</div>

            <div className="block lg:hidden ml-auto py-2">
              <Button
                  onClick={toggleDrawer(true)}
                  className="rounded-md bg-white px-3 py-2 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
              >
                <MenuSharp/>
              </Button>
              <Drawer open={toggleMenu} onClose={toggleDrawer(false)}>
                {MenuList()}
              </Drawer>
            </div>
          </div>
          <div className="bg-app-dark-blue min-h-1.5"/>
        </div>
        <div className={`w-full top-0 z-49`} style={{height: height ?? 80}}/>
      </div>
  );
};