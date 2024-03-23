using PrivateCA.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCA.Core.CA {
    public interface IPrivateCAApi {
        public Task<CsrResponseDTO> SignCsrAsync(CsrDTO data);
    }
}
