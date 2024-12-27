using DevExpress.Utils.OAuth;
using Microsoft.IdentityModel.Tokens;
using SMG.Logging;
using SMG.TokenManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.Plugins.Login
{
    public partial class frmLogin : Form
    {
        
        private const string SecretKey = "your_super_secret_key_1234567890";
        public frmLogin()
        {
            InitializeComponent();
            if (TokenManager.TokenManager.IsLoggedIn())
            {
                MessageBox.Show("Bạn đã đăng nhập!");
                this.Close();
            }

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtUsername.Text == "1" && txtPassword.Text == "1")
                {
                    TokenManager.TokenManager.Token = GenerateJwtToken(txtUsername.Text);
                    TokenManager.TokenManager.TokenExpiry = DateTime.Now.AddDays(7); 

                    MessageBox.Show("Đăng nhập thành công!");
                    this.Close(); 
                }
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private string GenerateJwtToken(string username)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Tạo payload cho JWT
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            };

                // Tạo token
                var token = new JwtSecurityToken(
                    issuer: "SMG", // Đặt tên ứng dụng
                    audience: "SMG",
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7), // Token hết hạn sau 7 ngày
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
                return null;
            }
        }
    }
}
