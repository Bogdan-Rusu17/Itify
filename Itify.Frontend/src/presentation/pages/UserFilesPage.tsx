import { WebsiteLayout } from "presentation/layouts/WebsiteLayout";
import { Fragment, memo } from "react";
import { Box } from "@mui/system";
import { Seo } from "@presentation/components/ui/Seo";
import { ContentCard } from "@presentation/components/ui/ContentCard";
import { UserFileTable } from "@presentation/components/ui/Tables/UserFileTable";

export const UserFilesPage = memo(() => {
  return <Fragment>
    <Seo title="MobyLab Web App | User Files" />
    <WebsiteLayout>
      <Box sx={{ padding: "0px 50px 00px 50px", justifyItems: "center" }}>
        <ContentCard>
          <UserFileTable />
        </ContentCard>
      </Box>
    </WebsiteLayout>
  </Fragment>
});
