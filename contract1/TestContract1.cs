using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace contract1;
public class TestContract1 : SmartContract
{
    const byte Prefix_ContractOwner = 0xFF;

    [DisplayName("_deploy")]
    public void Deploy(object data, bool update)
    {
        if (update) return;

        var tx = (Transaction)Runtime.ScriptContainer;
        var key = new byte[] { Prefix_ContractOwner };
        Storage.Put(Storage.CurrentContext, key, tx.Sender);
    }

    public static void Update(ByteString nefFile, string manifest)
    {
        var key = new byte[] { Prefix_ContractOwner };
        var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, key);
        var tx = (Transaction)Runtime.ScriptContainer;

        if (!contractOwner.Equals(tx.Sender))
        {
            throw new Exception("Only the contract owner can update the contract");
        }

        ContractManagement.Update(nefFile, manifest, null);
    }
}
