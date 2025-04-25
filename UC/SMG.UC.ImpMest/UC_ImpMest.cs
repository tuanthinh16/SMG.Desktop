using SMG.Logging;
using SMG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace SMG.UC.ImpMest
{
    public partial class UC_ImpMest : UserControl
    {
        List<Product>listCurrentProduct = new List<Product>();
        List<ProductType> listProductType = new List<ProductType>();
        List<SMG.Models.ImpMest> listSelected = new List<SMG.Models.ImpMest>();
        private long? selectedProductID = null;
        public UC_ImpMest()
        {
            InitializeComponent();
        }

        private void UC_ImpMest_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataProduct();
                LoadDataProductType();
                btnImport.Enabled = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void LoadDataProductType()
        {
            try
            {
                SMG.DB.Helpper.ProductTypeHelper productTypeHelper = new SMG.DB.Helpper.ProductTypeHelper();
                listProductType = await productTypeHelper.FetchProductTypesAsync();
                if (listProductType != null)
                {
                    cboType.Properties.DataSource = listProductType;
                    cboType.Properties.ValueMember = "ID";
                    cboType.Properties.DisplayMember = "PRODUCT_TYPE_NAME";
                    cboType.Properties.View.Columns.Clear();
                    cboType.Properties.View.Columns.AddField("ID").Visible = false; // Ẩn cột ID nếu không cần hiển thị
                    cboType.Properties.View.Columns.AddField("PRODUCT_TYPE_NAME").Visible = true; // Hiển thị cột Name

                    // Thiết lập các thuộc tính của view nếu cần
                    cboType.Properties.View.OptionsView.ShowColumnHeaders = false; // Ẩn tiêu đề cột nếu không cần
                    cboType.Properties.View.OptionsView.ShowIndicator = false; // Ẩn chỉ báo dòng
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void LoadDataProduct()
        {
            try
            {
                SMG.DB.Helpper.ProductHelper productHelper = new SMG.DB.Helpper.ProductHelper();
                listCurrentProduct = await productHelper.FetchProductsAsync();
                if(listCurrentProduct != null)
                {
                    gridControlProduct.DataSource = listCurrentProduct;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if(selectedProductID == null)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần nhập");
                    return;
                }
                if (!Validation.Validation.ValiadtionRequiredControrl(txtBillNumber, dxErrorProvider1) || !Validation.Validation.ValiadtionRequiredControrl(txtSupplier, dxErrorProvider1)) return;
                listSelected.Add(new SMG.Models.ImpMest
                {
                    PRODUCT_ID = selectedProductID.Value,
                    AMOUNT = Decimal.Parse(txtAmount.Text),
                    VAT = Decimal.Parse(txtVAT.Text),
                    REQUEST_LOGINNAME = txtUsername.Text,
                    SUPPLIER = txtSupplier.Text,
                    BILL_NUMBER = txtBillNumber.Text,
                    IMP_TIME = SMG.DateTimeHelpper.Convert.DateTimeToTimeNumber(DateTime.Now),
                    IMP_MEST_CODE = Guid.NewGuid().ToString(),
                    IMP_MEST_TYPE_ID = "1",
                    PRODUCT_TYPE_ID = listCurrentProduct.FirstOrDefault(p => p.ID == selectedProductID.Value)?.PRODUCT_TYPE_ID ?? 0,
                    IMP_MEST_STT_ID = "1",

                });
                gridControlItemAdded.DataSource = listSelected;
                gridControlItemAdded.RefreshDataSource();
                btnImport.Enabled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private async void btnImport_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                if (this.listSelected.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần nhập");
                    return;
                }

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    SMG.DB.Helpper.ImpMestHelper impMestHelper = new SMG.DB.Helpper.ImpMestHelper();
                    foreach (var item in listSelected)
                    {
                        var rs = await impMestHelper.AddImpMestAsync(item);
                        if (!rs.Item1)
                        {
                            MessageBox.Show("Có lỗi xảy ra khi thêm sản phẩm " + item.PRODUCT_ID);
                            return;
                        }
                    }

                    // Complete the transaction if all operations succeed
                    transaction.Complete();
                    MessageBox.Show("Nhập hàng thành công");
                    listSelected.Clear();
                    gridControlItemAdded.DataSource = listSelected;
                    gridControlItemAdded.RefreshDataSource();
                    btnImport.Enabled = false;
                    selectedProductID = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                MessageBox.Show("Đã xảy ra lỗi. Các thay đổi đã được hoàn tác.");
            }
        }


        private void gridViewProduct_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound  && e.IsGetData)
                    
                {
                    if (e.IsGetData)
                    {
                        if(e.Column.FieldName == "PRODUCT_TYPE_NAME")
                        {
                            var product = (Product)e.Row;
                            var productType = listProductType.FirstOrDefault(pt => pt.ID == product.PRODUCT_TYPE_ID);
                            e.Value = productType?.PRODUCT_TYPE_NAME;
                        }
                        if(e.Column.FieldName  == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewProduct_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = gridViewProduct.GetRow(e.RowHandle) as Product;
                if (data != null)
                {
                    txtProduct.Text = data.PRODUCT_NAME;
                    selectedProductID = data.ID;
                    txtAmount.Text = data.AMOUNT.ToString();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewItemAdded_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound && e.IsGetData)

                {
                    if (e.IsGetData)
                    {
                        
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if(e.Column.FieldName == "IMP_TIME_STR")
                        {
                            var impMest = (SMG.Models.ImpMest)e.Row;
                            e.Value = SMG.DateTimeHelpper.Convert.TimeNumberToDateTime(impMest.IMP_TIME ?? 0).Value.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        if(e.Column.FieldName == "PRODUCT_NAME")
                        {
                            var impMest = (SMG.Models.ImpMest)e.Row;
                            var product = listCurrentProduct.FirstOrDefault(p => p.ID == impMest.PRODUCT_ID);
                            e.Value = product?.PRODUCT_NAME;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewItemAdded_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                //Nut xoa san pham da them
                var data = gridViewItemAdded.GetFocusedRow() as SMG.Models.ImpMest;
                if (data != null)
                {
                    listSelected.Remove(data);
                    gridControlItemAdded.DataSource = listSelected;
                    gridControlItemAdded.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
