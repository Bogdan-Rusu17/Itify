import { WebsiteLayout } from "presentation/layouts/WebsiteLayout";
import { Typography } from "@mui/material";
import { Fragment, memo } from "react";
import { useIntl } from "react-intl";
import { Seo } from "@presentation/components/ui/Seo";
import { ContentCard } from "@presentation/components/ui/ContentCard";

export const HomePage = memo(() => {
  const { formatMessage } = useIntl();

  return <Fragment>
      <Seo title="MobyLab Web App | Home" />
      <WebsiteLayout>
        <div className="pl-[50px] pr-[50px]">
          <ContentCard title={formatMessage({ id: "globals.welcome" })}>
            <Typography>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut tristique et egestas quis ipsum suspendisse. At volutpat diam ut venenatis tellus in metus vulputate eu. Diam maecenas ultricies mi eget mauris. Diam sollicitudin tempor id eu nisl nunc mi. Sagittis aliquam malesuada bibendum arcu vitae elementum curabitur vitae nunc. Sapien nec sagittis aliquam malesuada bibendum arcu vitae. Tortor id aliquet lectus proin nibh nisl. Et malesuada fames ac turpis egestas maecenas pharetra convallis posuere. Volutpat blandit aliquam etiam erat velit scelerisque in. Feugiat in fermentum posuere urna nec. Morbi tristique senectus et netus et. Faucibus purus in massa tempor nec feugiat nisl. Sit amet mauris commodo quis imperdiet massa tincidunt. Mauris rhoncus aenean vel elit scelerisque. Urna porttitor rhoncus dolor purus non enim praesent elementum. Massa enim nec dui nunc mattis enim ut tellus. Id eu nisl nunc mi ipsum faucibus vitae aliquet. Ac auctor augue mauris augue neque gravida in. Sollicitudin ac orci phasellus egestas.

              Elementum sagittis vitae et leo duis ut diam quam nulla. Elementum nisi quis eleifend quam adipiscing vitae. Ultrices vitae auctor eu augue ut lectus arcu bibendum. Senectus et netus et malesuada. Pharetra sit amet aliquam id. Elit scelerisque mauris pellentesque pulvinar pellentesque habitant morbi tristique senectus. Vitae congue eu consequat ac felis donec et odio pellentesque. Adipiscing elit pellentesque habitant morbi. Duis ultricies lacus sed turpis tincidunt id aliquet risus. Leo vel fringilla est ullamcorper eget.

              Lacus sed viverra tellus in hac. Eu sem integer vitae justo eget magna fermentum iaculis. Consequat id porta nibh venenatis cras sed. Sed elementum tempus egestas sed. Tincidunt dui ut ornare lectus sit amet. Facilisi morbi tempus iaculis urna id volutpat lacus laoreet. Duis at consectetur lorem donec massa sapien. Diam vulputate ut pharetra sit amet aliquam id. Senectus et netus et malesuada fames ac. Ultrices eros in cursus turpis massa. Commodo odio aenean sed adipiscing diam. Aliquet bibendum enim facilisis gravida neque convallis. Feugiat vivamus at augue eget. Augue lacus viverra vitae congue eu consequat ac felis donec. Magna eget est lorem ipsum dolor.

              In pellentesque massa placerat duis. Facilisis gravida neque convallis a cras semper auctor neque. Nisi lacus sed viverra tellus. Integer vitae justo eget magna fermentum iaculis eu non. Dictum sit amet justo donec enim diam vulputate ut. Semper feugiat nibh sed pulvinar proin gravida hendrerit lectus a. Tortor posuere ac ut consequat semper viverra nam libero justo. Dictum at tempor commodo ullamcorper a. Sit amet commodo nulla facilisi nullam vehicula ipsum a arcu. Luctus venenatis lectus magna fringilla. Posuere sollicitudin aliquam ultrices sagittis orci. Tortor posuere ac ut consequat semper viverra nam libero. Massa ultricies mi quis hendrerit dolor magna. Ac tincidunt vitae semper quis lectus nulla. Pretium fusce id velit ut tortor pretium viverra. Enim ut tellus elementum sagittis. Viverra tellus in hac habitasse platea dictumst vestibulum rhoncus est. Sed turpis tincidunt id aliquet risus. In vitae turpis massa sed. Sit amet risus nullam eget felis.

              Aliquet porttitor lacus luctus accumsan tortor posuere ac ut consequat. Non curabitur gravida arcu ac tortor dignissim convallis. Ac placerat vestibulum lectus mauris ultrices eros. Ullamcorper sit amet risus nullam eget felis. Quam lacus suspendisse faucibus interdum posuere. Viverra orci sagittis eu volutpat odio facilisis mauris sit. Sit amet nisl suscipit adipiscing bibendum. Nullam vehicula ipsum a arcu cursus vitae congue mauris rhoncus. Est velit egestas dui id ornare arcu. Gravida cum sociis natoque penatibus et magnis. Massa tincidunt dui ut ornare lectus sit amet est. Nec nam aliquam sem et tortor. Suspendisse ultrices gravida dictum fusce. Ac turpis egestas sed tempus urna et. Scelerisque in dictum non consectetur a erat. Curabitur vitae nunc sed velit. Sit amet commodo nulla facilisi nullam vehicula. Blandit volutpat maecenas volutpat blandit aliquam. Eu tincidunt tortor aliquam nulla facilisi cras fermentum odio eu.

            </Typography>
          </ContentCard>
        </div>
      </WebsiteLayout>
    </Fragment>
});
